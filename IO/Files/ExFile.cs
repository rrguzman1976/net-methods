using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;

namespace ExamRef.ExamLib.IO.Files
{
    public class ExFile
    {
        public void Ex1_DriveInfo()
        {
            DriveInfo[] drivesInfo = DriveInfo.GetDrives();

            foreach (DriveInfo driveInfo in drivesInfo)
            {
                Console.WriteLine("Drive {0}", driveInfo.Name);
                Console.WriteLine("  File type: {0}", driveInfo.DriveType);

                if (driveInfo.IsReady)
                {
                    Console.WriteLine("  Volume label: {0}", driveInfo.VolumeLabel);
                    Console.WriteLine("  File system: {0}", driveInfo.DriveFormat);
                    Console.WriteLine("  Available space to current user:{0, 15} bytes",
                        driveInfo.AvailableFreeSpace);

                    Console.WriteLine("  Total available space:          {0, 15} bytes",
                        driveInfo.TotalFreeSpace);

                    Console.WriteLine("  Total size of drive:            {0, 15} bytes ",
                        driveInfo.TotalSize);
                }
            }
        }

        public void Ex2_CreateDir()
        {
            string path = @"..\..\..\TmpFile";
            string dir = Path.Combine(path, @"Dir1");

            if (!Directory.Exists(dir))
            {
                Console.WriteLine("Creating directory: {0}", dir);
                var directory = Directory.CreateDirectory(dir);

                var directoryInfo = new DirectoryInfo(dir);
                directoryInfo.Create();
            }
        }

        public void Ex3_SetDirPermit()
        {
            string path = @"..\..\..\TmpFile";
            string dir = Path.Combine(path, @"Dir1");

            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            directoryInfo.Create();

            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.AddAccessRule(
                        new FileSystemAccessRule("everyone",
                                                 FileSystemRights.ReadAndExecute,
                                                 AccessControlType.Allow));
            directoryInfo.SetAccessControl(directorySecurity);
        }

        public void Ex3_RemoveDirPermit()
        {
            string path = @"..\..\..\TmpFile";
            string dir = Path.Combine(path, @"Dir1");

            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            //directoryInfo.Create();

            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.RemoveAccessRuleSpecific(
                        new FileSystemAccessRule("everyone",
                                                 FileSystemRights.ReadAndExecute,
                                                 AccessControlType.Allow));
            directoryInfo.SetAccessControl(directorySecurity);
        }

        public void Ex4_ListDirectories()
        {
            string path = @"C:\Users\rguzman\Desktop\Personal";

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            ListDirectories(directoryInfo, "*", 5, 0);

            // OR
            DirectoryInfo[] subDirectories = directoryInfo.GetDirectories("*", SearchOption.AllDirectories);

            foreach (DirectoryInfo subDirectory in subDirectories)
            {
                Console.WriteLine(">>" + subDirectory.Name);
            }

        }

        private void ListDirectories(DirectoryInfo directoryInfo,
            string searchPattern, int maxLevel, int currentLevel)
        {
            if (currentLevel >= maxLevel)
            {
                return;
            }

            string indent = new string('-', currentLevel);

            try
            {
                DirectoryInfo[] subDirectories = directoryInfo.GetDirectories(searchPattern);

                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    Console.WriteLine(indent + subDirectory.Name);
                    ListDirectories(subDirectory, searchPattern, maxLevel, currentLevel + 1);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // You don't have access to this folder. 
                Console.WriteLine(indent + "Can't access: " + directoryInfo.Name);
                return;
            }
            catch (DirectoryNotFoundException)
            {
                // The folder is removed while iterating
                Console.WriteLine(indent + "Can't find: " + directoryInfo.Name);
                return;
            }
        }

        public void Ex5_MoveDirectory()
        {
            string path = @"..\..\..\TmpFile";
            string dir = Path.Combine(path, @"Dir1");

            string path2 = @"..\..\..\TmpDat";
            string dir2 = Path.Combine(path2, @"Dir1");

            Directory.Move(dir, dir2);

            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            directoryInfo.MoveTo(dir2);
        }

        public void Ex6_ListFiles()
        {
            string path = @"..\..\..\TmpFile";

            foreach (string file in Directory.GetFiles(path))
            {
                Console.WriteLine(file);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                Console.WriteLine(fileInfo.FullName);
            }
        }

        public void Ex7_DeleteFile()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"tracefile.txt");

            if (File.Exists(path)) // file exists
            {
                // Use exception handling to anticpate race conditions.
                try
                {
                    // File deleted by another thread here.
                    File.Delete(path);
                }
                catch (FileNotFoundException e) 
                {
                    Console.WriteLine("{0}", e.Message);
                }
            }

            FileInfo fileInfo = new FileInfo(path);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        public void Ex8_MoveFile()
        {
            string path = @"..\..\..\TmpFile\tracefile.txt";
            string destPath = @"..\..\..\TmpDat\tracefile.txt";

            File.CreateText(path).Close();
            File.Move(path, destPath);

            FileInfo fileInfo = new FileInfo(destPath);
            fileInfo.MoveTo(path);
        }

        public void Ex9_CopyFile()
        {
            string path = @"..\..\..\TmpFile\tracefile.txt";
            string destPath = @"..\..\..\TmpDat\tracefile.txt";

            File.CreateText(path).Close();
            File.Copy(path, destPath);

            FileInfo fileInfo = new FileInfo(path);
            fileInfo.CopyTo(destPath);
        }

        public void Ex10_Path()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"tracefile.txt");

            Console.WriteLine(Path.GetDirectoryName(file)); // Displays C:\temp\subdir
            Console.WriteLine(Path.GetExtension(file)); // Displays .txt
            Console.WriteLine(Path.GetFileName(file)); // Displays file.txt
            Console.WriteLine(Path.GetPathRoot(file)); // Displays C:\
        }

        public void Ex11_TmpFile()
        {
            // To: C:\Users\rguzman\AppData\Local\Temp\
            Console.WriteLine("Creating temporary random file: {0}", Path.GetTempFileName());
        }
    }
}
