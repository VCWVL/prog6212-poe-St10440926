using System.Security.Cryptography;
using System.Text;

namespace st10440926_poeparttwo.Models
{
    public static class EncryptionHelper
    {
        private static readonly string Key = "POE2025_SECURE_KEY_FOR_CMCS_APP";

        // Encrypt a file and save it as .enc
        public static void EncryptFile(string inputPath, string outputPath)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key.PadRight(32).Substring(0, 32));
            aes.GenerateIV();

            using FileStream inputFile = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            using FileStream outputFile = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

            // Write IV first
            outputFile.Write(aes.IV, 0, aes.IV.Length);

            using CryptoStream cryptoStream = new CryptoStream(outputFile, aes.CreateEncryptor(), CryptoStreamMode.Write);
            inputFile.CopyTo(cryptoStream);
        }

        // Decrypt a file and save the readable version
        public static void DecryptFile(string inputPath, string outputPath)
        {
            using FileStream inputFile = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key.PadRight(32).Substring(0, 32));

            byte[] iv = new byte[16];
            inputFile.Read(iv, 0, iv.Length);
            aes.IV = iv;

            using FileStream outputFile = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            using CryptoStream cryptoStream = new CryptoStream(inputFile, aes.CreateDecryptor(), CryptoStreamMode.Read);
            cryptoStream.CopyTo(outputFile);
        }

        // Optional: encrypt all files in a folder
        public static void EncryptAllInFolder(string sourceFolder, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);

            foreach (var file in Directory.GetFiles(sourceFolder))
            {
                string fileName = Path.GetFileName(file);
                string outputPath = Path.Combine(destinationFolder, fileName + ".enc");
                EncryptFile(file, outputPath);
            }
        }

        // Optional: decrypt all encrypted files in a folder
        public static void DecryptAllInFolder(string sourceFolder, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);

            foreach (var file in Directory.GetFiles(sourceFolder, "*.enc"))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string outputPath = Path.Combine(destinationFolder, fileName);
                DecryptFile(file, outputPath);
            }
        }
    }
}
