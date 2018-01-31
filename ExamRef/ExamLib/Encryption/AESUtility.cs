using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace ExamRef.ExamLib.Encryption
{
    // Utility class which implements AES encryption (symmetric)
    // Caches IV and Key to database.
    public sealed class AESUtility
    {
        public static byte[] Encrypt(string clearText, string cnxString)
        {
            using (AesManaged aes = new AesManaged())
            {
                byte[] tmpKey;
                byte[] tmpIV;

                AESUtilityInfo p = ReadParams(cnxString: cnxString);

                if (p == null)
                {
                    tmpKey = aes.Key;
                    tmpIV = aes.IV;

                    PersistParams(aes.Key, aes.IV, cnxString: cnxString);
                }
                else
                {
                    tmpKey = p.Key;
                    tmpIV = p.IV;
                }

                ICryptoTransform encryptor = aes.CreateEncryptor(tmpKey, tmpIV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) // utf-8
                        {
                            swEncrypt.Write(clearText);
                        }

                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        public static string Decrypt(byte[] cypherText, string cnxString)
        {
            using (AesManaged aes = new AesManaged())
            {
                AESUtilityInfo p = ReadParams(cnxString: cnxString);

                ICryptoTransform decryptor = aes.CreateDecryptor(p.Key, p.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cypherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader swDecrypt = new StreamReader(csDecrypt)) // utf-8
                        {
                            return swDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public void RegenerateParams()
        {
            throw new NotImplementedException("TODO: Implement method to regenerate Key and IV.");
        }

        // Save to database for reuse.
        private static void PersistParams(byte[] Key, byte[] IV, string cnxString)
        {
            // Write to db as binary.
            string cnxnString = ConfigurationManager.ConnectionStrings[cnxString].ConnectionString;

            using (var conxn = new SqlConnection(cnxnString))
            {
                conxn.Open();

                string cmd = @"INSERT INTO dbo.AESParameters ([ID], [IV], [KEY])
                                VALUES (@pID , @pIV, @pKey)";

                SqlCommand sc = new SqlCommand(cmd, conxn);
                sc.Parameters.AddWithValue("@pID", "AESManaged");
                sc.Parameters.AddWithValue("@pIV", IV);
                sc.Parameters.AddWithValue("@pKEY", Key);

                int r = sc.ExecuteNonQuery();
            }
        }

        private static AESUtilityInfo ReadParams(string cnxString)
        {
            string cnxnString = ConfigurationManager.ConnectionStrings[cnxString].ConnectionString;

            using (var conxn = new SqlConnection(cnxnString))
            {
                conxn.Open();

                string qry = @"SELECT	[KEY], [IV]
                                FROM	dbo.AESParameters
                                WHERE	[ID] = @pID
                                ORDER BY ID
                                OFFSET 0 ROWS FETCH NEXT 1 ROW ONLY";

                SqlCommand sc = new SqlCommand(qry, conxn);
                sc.Parameters.AddWithValue("@pID", "AESManaged");

                SqlDataReader r = sc.ExecuteReader();

                if (r.Read())
                {
                    return new AESUtilityInfo
                        {
                            Key = (byte[])r["KEY"],
                            IV = (byte[])r["IV"]
                        };
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public class AESUtilityInfo
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
    }
}
