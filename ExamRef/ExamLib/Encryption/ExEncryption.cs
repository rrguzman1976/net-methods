using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace ExamRef.ExamLib.Encryption
{
    public class ExEncryption
    {
        public void Ex1_AesManaged()
        {
            string original = "My secret data!";

            using (SymmetricAlgorithm symmetricAlgorithm = new AesManaged())
            {
                byte[] encrypted = Encrypt(symmetricAlgorithm, original);

                string roundtrip = Decrypt(symmetricAlgorithm, encrypted);

                Console.WriteLine("Original:   {0}", original);
                Console.WriteLine("Round Trip: {0}", roundtrip);
            }
        }

        public byte[] Encrypt(SymmetricAlgorithm aesAlg, string plainText)
        {
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        public string Decrypt(SymmetricAlgorithm aesAlg, byte[] cipherText)
        {
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        public void Ex2_RSAAsymmetric()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string publicKeyXML = rsa.ToXmlString(false);
            string privateKeyXML = rsa.ToXmlString(true);

            Console.WriteLine("Public key: {0}", publicKeyXML);
            Console.WriteLine("Private key: {0}", privateKeyXML);

            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = ByteConverter.GetBytes("My Secret Data!");

            byte[] encryptedData;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromXmlString(publicKeyXML);
                encryptedData = RSA.Encrypt(dataToEncrypt, false);
            }

            byte[] decryptedData;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromXmlString(privateKeyXML);
                decryptedData = RSA.Decrypt(encryptedData, false);
            }

            string decryptedString = ByteConverter.GetString(decryptedData);
            Console.WriteLine(decryptedString); // Displays: My Secret Data31!
        }

        public void Ex3_RSASafe()
        {
            string containerName = "SecretContainer";
            CspParameters csp = new CspParameters() { KeyContainerName = containerName };

            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = ByteConverter.GetBytes("My Secret Data!");

            byte[] encryptedData;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(csp))
            {
                encryptedData = RSA.Encrypt(dataToEncrypt, false);
            }

            byte[] decryptedData;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(csp))
            {
                decryptedData = RSA.Decrypt(encryptedData, false);
            }

            string decryptedString = ByteConverter.GetString(decryptedData);
            Console.WriteLine(decryptedString); // Displays: My Secret Data31!
        }
    }
}
