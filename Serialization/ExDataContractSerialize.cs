using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Serialization
{
    [DataContract]
    public class Person13
    {
        public Person13(): this(null)
        {
            ;
        }

        public Person13(string ssn)
        {
            SSN = ssn;
        }

        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        private string SSN { get; set; }
    }

    public class ExDataContractSerialize
    {
        public void Ex1_DataContract()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"Person13.xml");

            // Serialize
            Person13 p1 = new Person13("SSN")
            {
                Name = "Ricardo"
            };

            using (FileStream writer = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(Person13));
                ser.WriteObject(writer, p1);
            }

            // Deserialize
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(Person13));

                    // Deserialize the data and read it from the instance.
                    Person13 tmp = (Person13)ser.ReadObject(reader, true);

                    Console.WriteLine("Reconstituted: {0}", tmp.Name);
                }
            }
        }

        public void Ex2_DataContractJson()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"Person13.json");

            // Serialize
            Person13 p1 = new Person13("SSN2")
            {
                Name = "Frank"
            };

            using (FileStream writer = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Person13));
                ser.WriteObject(writer, p1);
            }

            // Deserialize
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Person13));

                // Deserialize the data and read it from the instance.
                Person13 tmp = (Person13)ser.ReadObject(fs);

                Console.WriteLine("Reconstituted: {0}", tmp.Name);
            }
        }
    }
}
