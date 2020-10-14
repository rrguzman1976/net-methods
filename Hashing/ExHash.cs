using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ExamRef.ExamLib.Hashing
{
    public class ExHash
    {
        public void Ex1_SHA256()
        {
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            SHA256 sha256 = SHA256.Create();

            string data = "A paragraph of text";
            byte[] hashA = sha256.ComputeHash(byteConverter.GetBytes(data));

            data = "a paragraph of text";
            byte[] hashB = sha256.ComputeHash(byteConverter.GetBytes(data));

            data = "A paragraph of text";
            byte[] hashC = sha256.ComputeHash(byteConverter.GetBytes(data));

            Console.WriteLine("hashA: ");
            foreach (byte b in hashA)
                Console.Write("{0}", b);
           
            Console.WriteLine(Environment.NewLine + "hashB");
            foreach (byte b in hashB)
                Console.Write("{0}", b);

            Console.WriteLine();
            Console.WriteLine(hashA.SequenceEqual(hashB)); // Displays: false
            Console.WriteLine(hashA.SequenceEqual(hashC)); // Displays: true
        }
    }
}
