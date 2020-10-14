using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;

namespace ExamRef.ExamLib.IO.Db
{
    public class ExDb
    {
        public void Ex1_Connect()
        {
            string conxnString = @"Server=localhost;Database=AdventureWorks2014;Trusted_Connection=True;";

            using (SqlConnection connection = new SqlConnection(conxnString))
            {
                try
                {
                    connection.Open();
                    // Execute operations against the database
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            } // Connection is automatically closed.
        }

        public void Ex2_ConnectDynamic()
        {
            var sqlConxnStringBuilder = new SqlConnectionStringBuilder();

            sqlConxnStringBuilder.DataSource = @"localhost";
            sqlConxnStringBuilder.InitialCatalog = "AdventureWorks2014";
            sqlConxnStringBuilder.IntegratedSecurity = true;

            string conxnString = sqlConxnStringBuilder.ToString();

            using (SqlConnection connection = new SqlConnection(conxnString))
            {
                try
                {
                    connection.Open();
                    // Execute operations against the database
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            } // Connection is automatically closed.
        }
    
        public void Ex3_ConnectConfig()
        {
            // App.config in ExamRef_Test
            string conxnString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(conxnString))
            {
                try
                {
                    connection.Open();
                    // Execute operations against the database
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            } // Connection is automatically closed.
        }

        public async Task Ex4_SelectAsync()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AW2017Local"].ConnectionString;
            string qry = @"SELECT	[BusinessEntityID]
		                            ,[FirstName]
		                            ,[MiddleName]
		                            ,[LastName]
                            FROM	[Person].[Person]
                            ORDER BY (SELECT NULL)
                            OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // blocks

                    SqlCommand command = new SqlCommand(qry, connection);

                    using (SqlDataReader dataReader = await command.ExecuteReaderAsync()) // blocks
                    {
                        while (await dataReader.ReadAsync()) // blocks
                        {
                            if ((dataReader["MiddleName"] == null))
                            {
                                Console.WriteLine("Person ({0}) is named {1} {3}",
                                    dataReader["BusinessEntityID"],
                                    dataReader["FirstName"],
                                    dataReader["LastName"]);
                            }
                            else
                            {
                                Console.WriteLine("Person ({0}) is named {1} {2} {3}",
                                    dataReader["BusinessEntityID"],
                                    dataReader["FirstName"],
                                    dataReader["MiddleName"],
                                    dataReader["LastName"]);
                            }
                        }
                        //dataReader.Close(); // called by dispose() automatically
                        Console.WriteLine("Async read complete.");
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task Ex5_SelectAsyncMultipleRS()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AW2014Local"].ConnectionString;

            string qry = @"SELECT	[BusinessEntityID]
		                            ,[FirstName]
		                            ,[MiddleName]
		                            ,[LastName]
                            FROM	[Person].[Person]
                            ORDER BY (SELECT NULL)
                            OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;
                            SELECT	[BusinessEntityID]
		                            ,[FirstName]
		                            ,[MiddleName]
		                            ,[LastName]
                            FROM	[Person].[Person]
                            ORDER BY (SELECT NULL)
                            OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand command = new SqlCommand(qry, connection);

                    SqlDataReader dataReader = await command.ExecuteReaderAsync();

                    await ReadQueryResults(dataReader);

                    await dataReader.NextResultAsync(); // Move to the next result set

                    Console.WriteLine("Next result set.");

                    await ReadQueryResults(dataReader);

                    dataReader.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task ReadQueryResults(SqlDataReader dataReader)
        {
            try
            {
                while (await dataReader.ReadAsync())
                {
                    if ((dataReader["MiddleName"] == null))
                    {
                        Console.WriteLine("Person ({0}) is named {1} {3}",
                            dataReader["BusinessEntityID"],
                            dataReader["FirstName"],
                            dataReader["LastName"]);
                    }
                    else
                    {
                        Console.WriteLine("Person ({0}) is named {1} {2} {3}",
                            dataReader["BusinessEntityID"],
                            dataReader["FirstName"],
                            dataReader["MiddleName"],
                            dataReader["LastName"]);
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task Ex6_InsertAsync()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;
            string cmd = @"INSERT INTO [dbo].[Test01] ([SVAL], [NVAL])
	                            VALUES ('Test 1', 1)
			                            , ('Test 2', 2)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(cmd, connection);

                await connection.OpenAsync();

                int numberOfUpdatedRows = await command.ExecuteNonQueryAsync();
                
                Console.WriteLine("Inserted {0} rows", numberOfUpdatedRows);
            }
        }

        public async Task Ex7_UpdateAsync()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;
            string cmd = @"UPDATE [dbo].[Test01]
                               SET [SVAL] += ' - UPDATE'
                                  ,[NVAL] += 1";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(cmd, connection);

                await connection.OpenAsync();

                int numberOfUpdatedRows = await command.ExecuteNonQueryAsync();

                Console.WriteLine("Updated {0} rows", numberOfUpdatedRows);
            }
        }

        // Use output param
        public void Ex8_SprocOutParam()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;
            string cmd = @"EXEC [dbo].[GetPerson2]
		                        @ID = @pID,
		                        @SVAL = @pSVAL OUTPUT;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(cmd, connection);
        
                command.Parameters.AddWithValue("@pID", "1");

                SqlParameter p1 = new SqlParameter();
                p1.ParameterName = "@pSVAL";
                p1.SqlDbType = SqlDbType.VarChar;
                p1.Size = 256;
                p1.Direction = ParameterDirection.Output;
                command.Parameters.Add(p1);

                command.ExecuteNonQuery();

                // Get output/return param values.
                string outval = (string)command.Parameters["@pSVAL"].Value;

                Console.WriteLine("Out value: {0}", outval);
            }
        }

        public async Task Ex8_SprocUpdateAsync()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;
            string cmd = @"EXEC dbo.UpdatePerson 
	                            @ID = @pID
	                            , @SVAL = @pSVAL
	                            , @NVAL = @pNVAL;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(cmd, connection);

                command.Parameters.AddWithValue("@pID", "1");
                command.Parameters.AddWithValue("@pSVAL", "Update from Code");
                command.Parameters.AddWithValue("@pNVAL", "4");

                int numberOfUpdatedRows = await command.ExecuteNonQueryAsync();

                Console.WriteLine("Updated {0} rows", numberOfUpdatedRows);
            }
        }

        // Save encrypted string to db.
        private byte[] _IV;
        private byte[] _Key;

        public void Ex9_SaveAESDb()
        {
            string clearText = "My secret data!";
            byte[] encryptedBytes;

            using (SymmetricAlgorithm aesAlg = new AesManaged())
            {
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // These should be persisted to db as well in a production impl. Otherwise,
                // each record is encrypted with its own IV and key.
                _IV = aesAlg.IV;
                _Key = aesAlg.Key;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) // UTF-8
                        {
                            swEncrypt.Write(clearText);
                        }

                        encryptedBytes = msEncrypt.ToArray();
                    }
                }
            }

            // A byte array can be written to a VARBINARY column.
            string connectionString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;
            string cmd = @"INSERT INTO [dbo].[Test01] ([SVAL], [BINVAL])
                            VALUES (@pSVAL , @pBINVAL);";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(cmd, connection);

                command.Parameters.AddWithValue("@pSVAL", "Write encrypted");
                command.Parameters.AddWithValue("@pBINVAL", encryptedBytes);

                int numberOfUpdatedRows = command.ExecuteNonQuery();

                Console.WriteLine("Updated {0} rows", numberOfUpdatedRows);
            }
        }

        // Load encrypted string from db.
        public void Ex9_LoadAESDb()
        {
            string clearText;
            byte[] cipherText = null;

            string connectionString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;
            string qry = @"SELECT	[BINVAL]
                            FROM	[dbo].[Test01]
                            WHERE   [SVAL] = @pSVAL
                            ORDER BY ID DESC
                            OFFSET 0 ROWS FETCH NEXT 1 ROW ONLY;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(qry, connection);
                command.Parameters.AddWithValue("pSVAL", "Write encrypted");

                SqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.Read())
                {
                    cipherText = (byte[]) dataReader["BINVAL"];
                }
                dataReader.Close();
            }

            using (SymmetricAlgorithm aesAlg = new AesManaged())
            {
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(_Key, _IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) // UTF-8
                        {
                            clearText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            Console.WriteLine("Secret: {0}", clearText);
        }

        public void Ex10_SaveRSADb()
        {
            // TODO: The data can only be decrypted from this machine. To make this code
            // portable, save the public and private keys to a database.
            string plainText = "my sensitive data!";

            RSAPersistKeyInCSP("RKOContainer2");

            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "RKOContainer2";

            byte[] encryptedData;

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(csp))
            {
                try
                {
                    byte[] dataToEncrypt = Encoding.Unicode.GetBytes(plainText);
                    encryptedData = RSA.Encrypt(dataToEncrypt, false);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
            }

            // A byte array can be written to a VARBINARY column.
            string connectionString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;
            string cmd = @"INSERT INTO [dbo].[Test01] ([SVAL], [BINVAL])
                            VALUES (@pSVAL , @pBINVAL);";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(cmd, connection);

                command.Parameters.AddWithValue("@pSVAL", "Write encrypted RSA");
                command.Parameters.AddWithValue("@pBINVAL", encryptedData);

                int numberOfUpdatedRows = command.ExecuteNonQuery();

                Console.WriteLine("Updated {0} rows", numberOfUpdatedRows);
            }

        }

        public void Ex10_LoadRSADb()
        {
            string clearText;
            byte[] cipherText = null;

            string connectionString = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;
            string qry = @"SELECT	[BINVAL]
                            FROM	[dbo].[Test01]
                            WHERE   [SVAL] = @pSVAL
                            ORDER BY ID DESC
                            OFFSET 0 ROWS FETCH NEXT 1 ROW ONLY;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(qry, connection);
                command.Parameters.AddWithValue("pSVAL", "Write encrypted RSA");

                SqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.Read())
                {
                    cipherText = (byte[])dataReader["BINVAL"];
                }
                dataReader.Close();
            }

            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "RKOContainer2";

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(csp))
            {
                try
                {
                    byte[] decryptedData = RSA.Decrypt(cipherText, false);
                    clearText = Encoding.Unicode.GetString(decryptedData);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
            }

            Console.WriteLine("Secret: {0}", clearText);

        }

        private void RSAPersistKeyInCSP(string ContainerName)
        {
            try
            {
                // Create a new instance of CspParameters.  Pass
                // 13 to specify a DSA container or 1 to specify
                // an RSA container.  The default is 1.
                CspParameters cspParams = new CspParameters();

                // Specify the container name using the passed variable.
                cspParams.KeyContainerName = ContainerName;

                //Create a new instance of RSACryptoServiceProvider to generate
                //a new key pair.  Pass the CspParameters class to persist the 
                //key in the container.
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider(cspParams);

                //Indicate that the key was persisted.
                Console.WriteLine("The RSA key was persisted in the container, \"{0}\".", ContainerName);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

            }
        }
    }
}
