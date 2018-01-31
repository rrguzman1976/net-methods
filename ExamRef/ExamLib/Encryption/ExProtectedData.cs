using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Encryption
{
    public class ExProtectedData
    {
        public byte[] Ex1_ProtectData()
        {
            string pw = "My secret password!";
            byte[] pwData = Encoding.UTF8.GetBytes(pw);
            byte[] safeData = null;

            try
            {
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                //  only by the same current user.
                safeData = ProtectedData.Protect(pwData, optionalEntropy: null, scope: DataProtectionScope.CurrentUser); // also LocalMachine
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not encrypted. An error occurred.");
                Console.WriteLine(e.ToString());
            }

            return safeData;
        }

        public void Ex2_UnprotectData(byte[] safeData)
        {
            try
            {
                //Decrypt the data using DataProtectionScope.CurrentUser.
                byte[] originalData = ProtectedData.Unprotect(safeData, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);

                Console.WriteLine("Original: {0}", Encoding.UTF8.GetString(originalData));
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not decrypted. An error occurred.");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
