using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ExamRef.ExamLib.Certificates
{
    public class ExCertificate
    {
        /*
         * Run from VS2013 developer command prompt
         * makecert -n "CN=ShermanFlan" -sr currentuser -ss testCertStore31
         */
        public void Ex1_UseCert()
        {
            string textToSign = "Test paragraph";
            byte[] signature = Sign(textToSign, "cn=ShermanFlan");
            
            // Uncomment this line to make the verification step fail
            //signature[0] = 0;
            Console.WriteLine(Verify(textToSign, signature));
        }

        public byte[] Sign(string text, string certSubject)
        {
            X509Certificate2 cert = GetCertificate();
            var rsa = (RSACryptoServiceProvider)cert.PrivateKey;
            byte[] hash = HashData(text);
            
            return rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
        }

        public bool Verify(string text, byte[] signature)
        {
            X509Certificate2 cert = GetCertificate();
            var rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;
            byte[] hash = HashData(text);
            
            return rsa.VerifyHash(hash,
                CryptoConfig.MapNameToOID("SHA1"),
                signature);
        }

        public byte[] HashData(string text)
        {
            HashAlgorithm hashAlgorithm = new SHA1Managed();

            byte[] data = Encoding.Unicode.GetBytes(text);
            byte[] hash = hashAlgorithm.ComputeHash(data);
            
            return hash;
        }

        public X509Certificate2 GetCertificate()
        {
            X509Store my = new X509Store("testCertStore31", StoreLocation.CurrentUser);
            my.Open(OpenFlags.ReadOnly);

            var certificate = my.Certificates[0];

            return certificate;
        }
    }
}
