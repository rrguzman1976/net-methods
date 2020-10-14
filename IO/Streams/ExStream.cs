using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Net.Http;

namespace ExamRef.ExamLib.IO.Streams
{
    public class ExStream
    {
        public void Ex1_CreateFileFromStream(string filename)
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, filename);

            using (FileStream fileStream = File.Create(file))
            { 
                string myValue = "MyValue3";
                byte[] data = Encoding.UTF8.GetBytes(myValue); // .NET default
                //byte[] data = Encoding.ASCII.GetBytes(myValue);
                //byte[] data = Encoding.BigEndianUnicode.GetBytes(myValue);
                //byte[] data = Encoding.Unicode.GetBytes(myValue);
                //byte[] data = Encoding.UTF7.GetBytes(myValue);
                //byte[] data = Encoding.UTF32.GetBytes(myValue);

                fileStream.Write(data, 0, data.Length);
            }
        }

        public void Ex2_ReadFileFromStream(string filename)
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, filename);

            using (FileStream fileStream = File.OpenRead(file))
            {
                byte[] data = new byte[fileStream.Length];

                for (int index = 0; index < fileStream.Length; index++)
                {
                    data[index] = (byte)fileStream.ReadByte();
                }

                Console.WriteLine(Encoding.UTF8.GetString(data)); // Displays: MyValue
            }
        }

        public void Ex3_CreateFile2()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"newfile2.txt");

            using (StreamWriter streamWriter = File.CreateText(file)) // utf-8 default
            {
                string myValue = "MyValue";
                streamWriter.Write(myValue);
            }
        }

        public void Ex4_ReadFile2()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"newfile2.txt");

            using (StreamReader streamWriter = File.OpenText(file)) // utf-8 default
            {
                Console.WriteLine(streamWriter.ReadLine()); // Displays: MyValue
            }
        }

        public void Ex5_ChainZip() // chain streams
        {
            string folder = @"..\..\..\TmpFile";
            string unzipFilePath = Path.Combine(folder, "uncompressed.dat");
            string zipFilePath = Path.Combine(folder, "compressed.gz");

            byte[] dataToCompress = Enumerable.Repeat((byte)'a', 1024 * 1024).ToArray();

            using (FileStream unzipFileStream = File.Create(unzipFilePath)) // utf-8 default
            {
                unzipFileStream.Write(dataToCompress, 0, dataToCompress.Length);
            }

            using (FileStream zipFileStream = File.Create(zipFilePath)) // utf-8 default
            {
                // GZipStream writes to file fs.
                using (GZipStream zipStream = new GZipStream(zipFileStream, CompressionMode.Compress))
                {
                    zipStream.Write(dataToCompress, 0, dataToCompress.Length);
                }
            }

            FileInfo unzipFile = new FileInfo(unzipFilePath);
            FileInfo zipFile = new FileInfo(zipFilePath);

            Console.WriteLine(unzipFile.Length); // Displays 1048576
            Console.WriteLine(zipFile.Length); // Displays 1052
        }

        public void Ex5_ChainUnzip() // chain streams
        {
            string folder = @"..\..\..\TmpFile";
            string unzipFilePath = Path.Combine(folder, "compressed.gz");

            byte[] dataToUnzip = new byte[1024 * 1024];

            using (FileStream zipFileStream = File.Open(unzipFilePath, FileMode.Open, FileAccess.Read)) // utf-8 default
            {
                // GZipStream writes to file fs.
                using (GZipStream zipStream = new GZipStream(zipFileStream, CompressionMode.Decompress))
                {
                    zipStream.Read(dataToUnzip, 0, dataToUnzip.Length);
                }
            }

            Console.WriteLine("Uncompressed: {0}", Encoding.UTF8.GetString(dataToUnzip)); // Displays aaa...
        }

        // Decompresses existing file to a new file.
        public void Ex5_ChainUnzipB() // chain streams
        {
            string folder = @"..\..\..\TmpFile";
            string zipFilePath = Path.Combine(folder, "compressed.gz");
            string unzipFilePath = Path.Combine(folder, "unzip.txt");

            // NOTE: Doesn't require size of buffer upfront.
            //byte[] dataToUnzip = new byte[1024 * 1024];

            using (FileStream zipFileStream = File.Open(zipFilePath, FileMode.Open, FileAccess.Read)) // utf-8 default
            {
                using (FileStream unzipFileStream = File.OpenWrite(unzipFilePath)) // utf-8 default
                {
                    // GZipStream reads from zip file.
                    using (GZipStream zipStream = new GZipStream(zipFileStream, CompressionMode.Decompress))
                    {
                        // GZipStream writes to unzip file fs.
                        zipStream.CopyTo(unzipFileStream);
                    }
                }
            }
        }

        // Chain streams - encrypt message to file.
        private byte[] _IV;
        private byte[] _key;

        public void Ex6_ChainEncryptAES()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, "aes1.dat");
            string plainText = "my sensitive data!";

            using (SymmetricAlgorithm aesAlg = new AesManaged())
            {
                _IV = aesAlg.IV;
                _key = aesAlg.Key;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(_key, _IV); // symmetric key

                using (FileStream fileStream = File.Create(file))
                {
                    using (CryptoStream csEncrypt = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainBin = Encoding.UTF8.GetBytes(plainText);
                        csEncrypt.Write(plainBin, 0, plainBin.Length);

                        // OR
                        /*
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) // utf-8 default
                        {
                            swEncrypt.Write(plainText);
                        }
                         */
                    }
                }
            }
        }

        public void Ex6b_ChainDecryptAES()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, "aes1.dat");
            string clearText;

            using (SymmetricAlgorithm aesAlg = new AesManaged())
            {
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(_key, _IV);

                using (FileStream fileStream = File.OpenRead(file))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] plainBin = new byte[18];
                        csDecrypt.Read(plainBin, 0, 18);

                        clearText = Encoding.UTF8.GetString(plainBin);

                        // OR, using StreamReader doesn't require buffer size upfront!
                        /*
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            clearText = srDecrypt.ReadToEnd();
                        }
                         */ 
                    }
                }

                Console.WriteLine("Original:   {0}", clearText);
            }
        }

        public void Ex7_ChainEncryptRSA()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, "rsa1.dat");
            string plainText = "my sensitive data!";

            RSAPersistKeyInCSP("RKOContainer1");

            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "RKOContainer1";

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(csp))
            {
                using (FileStream fStream = File.Create(file))
                {
                    try
                    {
                        byte[] dataToEncrypt = Encoding.Unicode.GetBytes(plainText);
                        byte[] encryptedData = RSA.Encrypt(dataToEncrypt, false);

                        fStream.Write(encryptedData, 0, encryptedData.Length);
                    }
                    catch(CryptographicException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        public void Ex7b_ChainDecryptRSA()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, "rsa1.dat");

            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "RKOContainer1";

            using (FileStream fStream = File.OpenRead(file))
            {
                // NOTE: This is how you get the proper array length prior to reading. 
                byte[] encryptedData = new byte[fStream.Length];

                fStream.Read(encryptedData, 0, (int) fStream.Length);

                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(csp))
                {
                    try
                    {
                        byte[] decryptedData = RSA.Decrypt(encryptedData, false);
                        Console.WriteLine(Encoding.Unicode.GetString(decryptedData));
                    }
                    catch (CryptographicException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
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

        public async Task Ex8_WriteAsync()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, "async1.dat");

            using (FileStream stream = new FileStream(file, FileMode.Create,
                FileAccess.Write, FileShare.None, 4096, useAsync: true))
            {
                string s = "Hello world!";
                byte[] data = Encoding.UTF8.GetBytes(s);

                Stopwatch sw = new Stopwatch();

                sw.Start();
                await stream.WriteAsync(data, 0, data.Length); // blocks on background thread
                sw.Stop();

                Console.WriteLine("Elapsed: {0}", sw.Elapsed.ToString());
            }
        }

        public async Task<int> Ex8b_ReadAsync()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, "async1.dat");

            using (FileStream fs = new FileStream(file, FileMode.Open,
                FileAccess.Read, FileShare.None, 4096, useAsync: true))
            {
                byte[] data = new byte[fs.Length];

                Stopwatch sw = new Stopwatch();

                sw.Start();
                int n = await fs.ReadAsync(data, 0, data.Length); // blocks on background thread
                sw.Stop();

                Console.WriteLine("Elapsed: {0}", sw.Elapsed.ToString());
                Console.WriteLine("Read: {0}", Encoding.UTF8.GetString(data));
                return n;
            }
        }

        public Task<string> Ex_ReadAsync()
        {
            HttpClient client = new HttpClient();
            Task<string> t = client.GetStringAsync("http://www.microsoft.com");
            return t;
        }
    }
}
