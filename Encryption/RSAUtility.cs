using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Configuration;

namespace ExamRef.ExamLib.Encryption
{
    // Utility class providing asymmetric encryption/decryption
    // capabilities.
    public sealed class RSAUtility
    {
        
        public static byte[] Encrypt(string clearText, string cnxnString)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                string publicKey;
                string privateKey;

                RSAUtilityInfo info = ReadParams(cnxnString);

                if (info == null)
                {
                    publicKey = rsa.ToXmlString(includePrivateParameters: false);
                    privateKey = rsa.ToXmlString(includePrivateParameters: true);

                    WriteParams(publicKey, privateKey, cnxnString);
                }
                else
                {
                    publicKey = info.PublicKey;
                    privateKey = info.PrivateKey;
                }

                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(clearText);

                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.FromXmlString(publicKey);
                    return RSA.Encrypt(dataToEncrypt, false);
                }
            }
        }

        public static string Decrypt(byte[] cipherText, string cnxnString)
        {

            byte[] decryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAUtilityInfo info = ReadParams(cnxnString);
                string publicKey;
                string privateKey;

                if (info == null) // shouldn't happen
                {
                    publicKey = rsa.ToXmlString(includePrivateParameters: false);
                    privateKey = rsa.ToXmlString(includePrivateParameters: true);

                    WriteParams(publicKey, privateKey, cnxnString);

                }
                else
                {
                    publicKey = info.PublicKey;
                    privateKey = info.PrivateKey;
                }

                rsa.FromXmlString(privateKey);
                decryptedData = rsa.Decrypt(cipherText, false);

                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        private static RSAUtilityInfo ReadParams(string cnxnString)
        {
            string cnxn = ConfigurationManager.ConnectionStrings[cnxnString].ConnectionString;

            using (SqlConnection c = new SqlConnection(cnxn))
            {
                c.Open();

                string qry = @"SELECT	[PublicKey], [PrivateKey]
                                FROM	[dbo].[RSAParameters]
                                WHERE	[ID] = @pID;";

                SqlCommand cmd = new SqlCommand(qry, c);
                cmd.Parameters.AddWithValue("@pID", "RSACryptoServiceProvider");

                SqlDataReader r = cmd.ExecuteReader();

                if (r.Read())
                {
                    return new RSAUtilityInfo
                    {
                        PublicKey = (string)r["PublicKey"],
                        PrivateKey = (string)r["PrivateKey"]
                    };
                }
                else
                {
                    return null;
                }

            }
        }

        private static void WriteParams(string PublicKey, string PrivateKey, string cnxnString)
        {
            string cnxn = ConfigurationManager.ConnectionStrings[cnxnString].ConnectionString;

            using (SqlConnection c = new SqlConnection(cnxn))
            {
                c.Open();

                string qry = @"INSERT INTO [dbo].[RSAParameters] ([ID], PublicKey, PrivateKey)
                                VALUES (@pID, @pPublicKey, @pPrivateKey);";

                SqlCommand cmd = new SqlCommand(qry, c);
                cmd.Parameters.AddWithValue("@pID", "RSACryptoServiceProvider");
                cmd.Parameters.AddWithValue("@pPublicKey", PublicKey);
                cmd.Parameters.AddWithValue("@pPrivateKey", PrivateKey);

                int r = cmd.ExecuteNonQuery();
            }
        }
    }

    public class RSAUtilityInfo
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
