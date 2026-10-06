using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DVLD.Presentation
{
    internal class clsUtil
    {
        public static string GenerateGUID()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static bool CreateFolderIfDoesNotExist(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                try
                {
                    Directory.CreateDirectory(folderPath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;//folder is already exist
        }

        public static string ReplaceFileNameWithGUID(string fileName)
        {
            //full file name. chanage your file name
            FileInfo fileInfo = new FileInfo(fileName);

            string ex = fileInfo.Extension;

            return GenerateGUID() + ex;
        }

        public static bool CopyImageToProjectImageFolder(ref string sourceFile)
        {
            string destinationFolder = @"C:\DVLD-People-Images\";

            if (!CreateFolderIfDoesNotExist(destinationFolder))
            {
                return false;
            }

            string destinationFile = destinationFolder + ReplaceFileNameWithGUID(sourceFile);
            try
            {
                File.Copy(sourceFile, destinationFile, true);
            }
            catch (IOException iox)
            {
                MessageBox.Show(iox.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            sourceFile = destinationFile;
            return true;
        }
        
        private static readonly string _FilePath =
        Path.Combine(Application.StartupPath, "RememberMe.txt");
        
        public static bool SaveToFile(string userName, string password)
        {
            try
            {
                string enCodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

                File.WriteAllLines(_FilePath, new string[]
                {
                    userName,
                    enCodedPassword
                });
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool Load(ref string userName,ref string password)
        {
            try
            {

                if (!File.Exists(_FilePath))
                    return false;

                string[] data = File.ReadAllLines(_FilePath);

                if (data.Length < 2)
                    return false;

                userName = data[0];

                password = Encoding.UTF8.GetString(Convert.FromBase64String(data[1]));
                
                return true;
            }
            catch
            {
                return false;
            }

        }
        public static string ComputeHash(string input)
        {
            //SHA is Secutred Hash Algorithm.
            // Create an instance of the SHA-256 algorithm
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash value from the UTF-8 encoded input string
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));


                // Convert the byte array to a lowercase hexadecimal string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        public static bool DeleteRememberMeFile()
        {
            try
            {
                if (File.Exists(_FilePath))
                    File.Delete(_FilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
