using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Data.SqlClient;
using System.Configuration;
using ExamRef.ExamLib.Encryption;

namespace ExamRef.ExamLib.Serialization
{
    [Serializable]
    public class Person2
    {
        public Person2(bool isDirty)
        {
            _isDirty = isDirty;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public bool Dirty
        {
            get
            {
                return _isDirty;
            }
        }

        [NonSerialized]
        private bool _isDirty;

        // Serialization callbacks.
        [OnSerializing()]
        internal void OnSerializingMethod(StreamingContext context)
        {
            Console.WriteLine("OnSerializing.");
        }

        [OnSerialized()]
        internal void OnSerializedMethod(StreamingContext context)
        {
            Console.WriteLine("OnSerialized.");
        }

        [OnDeserializing()]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            Console.WriteLine("OnDeserializing.");
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Console.WriteLine("OnSerialized.");
        }
    }

    // Implement ISerializable for fine-grained control over the serialization
    // process.
    [Serializable]
    public class PersonComplex : ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SSN { get; set; }
        private bool isDirty = false;

        public PersonComplex() { }
        protected PersonComplex(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("Id");
            Name = info.GetString("Name");
            SSN = info.GetString("SSN");

            isDirty = info.GetBoolean("isDirty");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Name", Name);
            info.AddValue("SSN", SSN);

            info.AddValue("isDirty", isDirty);
        }
    }

    // Implement ISerializable for fine-grained control over the serialization
    // process.
    [Serializable]
    public class PersonComplexAES : ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SSN { get; set; }
        private byte[] SSNEncrypt { get; set; }
        private bool isDirty = false;
        private const string cnxString = "ScratchDBLocal";

        public PersonComplexAES() { }
        protected PersonComplexAES(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("Id");
            Name = info.GetString("Name");

            // Decrypt using utility class
            SSNEncrypt = (byte[]) info.GetValue("SSNEncrypt", typeof(byte[]));
            SSN = AESUtility.Decrypt(SSNEncrypt, cnxString);

            isDirty = info.GetBoolean("isDirty");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Name", Name);

            // Encrypt using utility class.
            //info.AddValue("SSN", SSN);
            byte[] data = AESUtility.Encrypt(SSN, cnxString);
            info.AddValue("SSNEncrypt", data);

            info.AddValue("isDirty", isDirty);
        }
    }

    // Implement ISerializable for fine-grained control over the serialization
    // process.
    [Serializable]
    public class PersonComplexRSA : ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SSN { get; set; }
        private byte[] SSNEncrypt { get; set; }
        private bool isDirty = false;
        private string cnxnString { get; set; }
        public PersonComplexRSA() { }
        protected PersonComplexRSA(SerializationInfo info, StreamingContext context, string cnxnString)
        {
            Id = info.GetInt32("Id");
            Name = info.GetString("Name");

            // Decrypt using utility class
            SSNEncrypt = (byte[])info.GetValue("SSNEncrypt", typeof(byte[]));
            SSN = RSAUtility.Decrypt(SSNEncrypt, cnxnString);
            this.cnxnString = cnxnString;

            isDirty = info.GetBoolean("isDirty");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Name", Name);

            // Encrypt using utility class.
            //info.AddValue("SSN", SSN);
            byte[] data = RSAUtility.Encrypt(SSN, cnxnString);
            info.AddValue("SSNEncrypt", data);

            info.AddValue("isDirty", isDirty);
        }
    }

    public class ExBinarySerialize
    {
        // Roundtrip binary serialization.
        public void Ex1_Serialize()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"Person2.dat");

            Person2 p = new Person2(true)
            {
                Id = 1,
                Name = "John Doe"
            };

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(file, FileMode.Create))
            {
                formatter.Serialize(stream, p);
            }

            using (Stream stream = new FileStream(file, FileMode.Open))
            {
                Person2 dp = (Person2) formatter.Deserialize(stream);

                Console.WriteLine("{0} {1} {2}", dp.Id, dp.Name, dp.Dirty);
            }
        }

        // Roundtrip binary serialization.
        public void Ex1b_SerializeCustom()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"PersonComplex.dat");

            PersonComplex p = new PersonComplex
            {
                Id = 1,
                Name = "John Doe",
                SSN = "1234567890"
            };

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(file, FileMode.Create))
            {
                formatter.Serialize(stream, p);
            }

            using (Stream stream = new FileStream(file, FileMode.Open))
            {
                PersonComplex dp = (PersonComplex) formatter.Deserialize(stream);

                Console.WriteLine("{0} {1} {2}", dp.Id, dp.Name, dp.SSN);
            }
        }

        // Binary serialization to DB. Use VARBINARY(MAX)
        public void Ex2_SerializeDB()
        {
            Person2 p = new Person2(true)
            {
                Id = 1,
                Name = "John Doe"
            };

            byte[] data;
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, p);
                data = stream.ToArray();
            }

            // Write to db as binary.
            string cnxnString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;

            using (var conxn = new SqlConnection(cnxnString))
            {
                conxn.Open();

                string cmd = @"INSERT INTO [dbo].[Test01] ([SVAL], [OBJVAL])
                                 VALUES ('Bin Serialize Test 1' , @pOBJVAL)";

                SqlCommand sc = new SqlCommand(cmd, conxn);
                sc.Parameters.AddWithValue("@pOBJVAL", data);

                int r = sc.ExecuteNonQuery();
            }
        }

        // Binary deserialization from DB.
        public void Ex2_DeserializeDB()
        {
            byte[] data = null;
            IFormatter formatter = new BinaryFormatter();

            string cnxnString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;

            using (var conxn = new SqlConnection(cnxnString))
            {
                conxn.Open();

                string qry = @"SELECT	[OBJVAL]
                                FROM	[dbo].[Test01]
                                WHERE	[SVAL] = @pSVAL
                                ORDER BY ID
                                OFFSET 0 ROWS FETCH NEXT 1 ROW ONLY";

                SqlCommand sc = new SqlCommand(qry, conxn);
                sc.Parameters.AddWithValue("@pSVAL", "Bin Serialize Test 1");

                SqlDataReader r = sc.ExecuteReader();

                if (r.Read())
                {
                    data = (byte[]) r["OBJVAL"];

                    using (MemoryStream stream = new MemoryStream(data))
                    {
                        Person2 dp = (Person2)formatter.Deserialize(stream);

                        Console.WriteLine("RT: {0} {1} {2}", dp.Id, dp.Name, dp.Dirty);
                    }
                }
            }
        }

        // Encrypt fields (use AES) when serializing.
        // Decrypt fields (use AES) when deserializing.
        public void Ex3_SerializeEncryptAES()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"PersonComplexAES.dat");

            PersonComplexAES p = new PersonComplexAES
            {
                Id = 1,
                Name = "John Doe",
                SSN = "1234567890"
            };

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(file, FileMode.Create))
            {
                formatter.Serialize(stream, p);
            }

            using (Stream stream = new FileStream(file, FileMode.Open))
            {
                PersonComplexAES dp = (PersonComplexAES) formatter.Deserialize(stream);

                Console.WriteLine("Decrypt: {0} {1} {2}", dp.Id, dp.Name, dp.SSN);
            }
        }

        // Encrypt fields (use RSA) when serializing.
        // Decrypt fields (use RSA) when deserializing.
        public void Ex4_SerializeEncryptRSA()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"PersonComplexRSA.dat");

            PersonComplexRSA p = new PersonComplexRSA
            {
                Id = 1,
                Name = "John Doe",
                SSN = "1234567890"
            };

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(file, FileMode.Create))
            {
                formatter.Serialize(stream, p);
            }

            using (Stream stream = new FileStream(file, FileMode.Open))
            {
                PersonComplexRSA dp = (PersonComplexRSA)formatter.Deserialize(stream);

                Console.WriteLine("Decrypt: {0} {1} {2}", dp.Id, dp.Name, dp.SSN);
            }
        }
    }
}
