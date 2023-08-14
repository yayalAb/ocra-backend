using Internal;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;


namespace AppDiv.CRVS.Application.Service
{
    public class FileExtractorService : IFileExtractorService
    {
        private readonly IEventImportService _eventImportService;
        public FileExtractorService(IEventImportService eventImportService)
        {
            _eventImportService = eventImportService;
        }


        private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>
        {
            {"image/bmp", "bmp"},
            {"image/cgm", "cgm"},
            {"image/vnd.djvu", "djv"},
            {"application/msword", "doc"},
            {"application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx"},
            {"application/vnd.openxmlformats-officedocument.wordprocessingml.template", "dotx"},
            {"application/vnd.ms-word.document.macroEnabled.12", "docm"},
            {"application/vnd.ms-word.template.macroEnabled.12", "dotm"},
            {"image/gif", "gif"},
            {"image/ief", "ief"},
            {"model/iges", "iges"},
            {"image/jp2", "jp2"},
            {"image/jpeg", "jpeg"},
            {"image/png", "png"},
            {"image/svg+xml", "svg"},
            {"image/tiff", "tiff"},
            {"image/vnd.wap.wbmp", "wbmp"},
            {"application/pdf", "pdf"},
            {"image/pict", "pct"},
            {"image/x-portable-anymap", "pnm"},
            {"image/x-macpaint", "pnt"},
            {"image/x-portable-pixmap", "ppm"},
            {"application/vnd.ms-powerpoint", "ppt"},
            {"application/vnd.openxmlformats-officedocument.presentationml.presentation", "pptx"},
            {"application/vnd.openxmlformats-officedocument.presentationml.template", "potx"},
            {"application/vnd.openxmlformats-officedocument.presentationml.slideshow", "ppsx"},
            {"application/vnd.ms-powerpoint.addin.macroEnabled.12", "ppam"},
            {"application/vnd.ms-powerpoint.presentation.macroEnabled.12", "pptm"},
            {"application/vnd.ms-powerpoint.template.macroEnabled.12", "potm"},
            {"application/vnd.ms-powerpoint.slideshow.macroEnabled.12", "ppsm"},
            {"image/x-quicktime", "qti"},
            {"application/postscript", "ps"},
            {"application/vnd.rn-realmedia", "rm"},
            {"application/vnd.ms-excel", "xls"},
            {"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx"},
            {"application/vnd.openxmlformats-officedocument.spreadsheetml.template", "xltx"},
            {"application/vnd.ms-excel.sheet.macroEnabled.12", "xlsm"},
            {"application/vnd.ms-excel.template.macroEnabled.12", "xltm"},
            {"application/vnd.ms-excel.addin.macroEnabled.12", "xlam"},
            {"application/vnd.ms-excel.sheet.binary.macroEnabled.12", "xlsb"},
            {"application/zip", "zip"}
            };
        private static readonly Dictionary<string, byte[]> fileMagicNumbers = new Dictionary<string, byte[]>
        {
            { "bmp", new byte[] { 0x42, 0x4D } },
            { "cgm", new byte[] { 0x00, 0x01, 0x00, 0x00, 0x48, 0x0A, 0x00, 0x00 } },
            { "djv", new byte[] { 0x41, 0x54, 0x26, 0x53 } },
            { "doc", new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } },
            { "docx", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "dotx", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "docm", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "dotm", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "gif", new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 } },
            { "ief", new byte[] { 0x42, 0x5A, 0x68 } },
            { "iges", new byte[] { 0x53, 0x50, 0x41, 0x43, 0x45, 0x20, 0x49, 0x47, 0x45, 0x53 } },
            { "jp2", new byte[] { 0x00, 0x00, 0x00, 0x0C, 0x6A, 0x50, 0x20, 0x20, 0x0D, 0x0A, 0x87, 0x0A } },
            { "jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },
            { "png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
            { "svg", new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65 } },
            { "tiff", new byte[] { 0x49, 0x49, 0x2A, 0x00 } },
            { "wbmp", new byte[] { 0x00, 0x00, 0x00, 0x0C } },
            { "pdf", new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D } },
            { "pct", new byte[] { 0x00, 0x11, 0xA7, 0x00 } },
            { "pnm", new byte[] { 0x50, 0x36 } },
            { "pnt", new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 } },
            { "ppm", new byte[] { 0x50, 0x36 } },
            { "ppt", new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } },
            { "pptx", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "potx", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "ppsx", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "ppam", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "pptm", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "potm", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "ppsm", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "qti", new byte[] { 0x00, 0x00, 0x00, 0x01 } },
            { "ps", new byte[] { 0x25, 0x21, 0x50, 0x53, 0x2D } },
            { "rm", new byte[] { 0x2E, 0x52, 0x4D, 0x46 } },
            { "xls", new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } },
            { "xlsx", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "xltx", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "xlsm", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "xltm", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "xlam", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "xlsb", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } },
            { "zip", new byte[] { 0x50, 0x4B, 0x03, 0x04 } }
        };


        public async Task<string[]> ExtractFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new NotFoundException("file not uploaded.");
            }
            string extractPath = Path.Combine(Directory.GetCurrentDirectory(), "ExtractedFiles");
            try
            {
                Directory.CreateDirectory(extractPath);
                string[] files = Directory.GetFiles(extractPath);

                // Loop through files and delete each one
                foreach (string fl in files)
                {
                    File.Delete(fl);
                }

                using (ZipArchive archive = new ZipArchive(file.OpenReadStream()))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string filePath = Path.Combine(extractPath, entry.FullName);
                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        }
                        else
                        {
                            entry.ExtractToFile(filePath, true);
                        }
                    }
                }
                string[] extractedFiles = Directory.GetFiles(extractPath);
                foreach (string item in extractedFiles)
                {
                    using (StreamReader reader = new StreamReader(item))
                    {
                        string contents = reader.ReadToEnd();
                        Console.WriteLine("Content encrepted : {0}",contents);
                        string content = Decrypt(contents, "OCRAOCRAOCRAOCRA");
                        Console.WriteLine("Content decrepted : {0}",content);
                        JArray jArray = JArray.Parse(content);
                          await _eventImportService.ImportEvent(jArray);

                    }


                }



                return extractedFiles;
            }
            catch (Exception ex)
            {
                throw new NotFoundException(ex.Message);
            }
        }

        public static string? GetFileExtensionFromBase64String(string base64String)
        {
            try
            {
                return GetExtensionFromByteSequence(base64String);
            }
            catch (System.Exception)
            {

                return null;
            }
            // Convert the base64 string to a byte array
        }

        public static string Decrypt(string cipherText, string key)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = new byte[16];
            using (SymmetricAlgorithm algorithm = Aes.Create())
            {
                algorithm.Mode = CipherMode.ECB;
                algorithm.Padding = PaddingMode.PKCS7;
                algorithm.Key = keyBytes;
                algorithm.IV = ivBytes;
                using (ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
        public static string? GetExtensionFromBase64Metadata(string base64String)
        {
            string[] parts = base64String.Split(',');
            if (parts.Length >= 2 && parts[0].StartsWith("data:") && parts[1].Contains("/"))
            {
                string mimeType = parts[0].Split(';')[0].Split(":")[1].Trim();
                return GetExtensionFromMimeType(mimeType);
            }

            // Default to an unknown file extension
            return null;
        }

        private static string? GetExtensionFromMimeType(string mimeType)
        {

            if (MIMETypesDictionary.ContainsKey(mimeType))
            {
                return "." + MIMETypesDictionary[mimeType];
            }
            return null;
        }
        public static string? GetExtensionFromByteSequence(string base64String)
        {
            if (!HelperService.IsBase64String(base64String))
            {
                return null;
            }
            base64String = base64String.Substring(base64String.IndexOf(',') + 1);
            byte[] fileBytes = Convert.FromBase64String(base64String);
            foreach (var magicNumber in fileMagicNumbers)
            {
                if (ByteArrayStartsWith(fileBytes, magicNumber.Value))
                {
                    return magicNumber.Key;
                }
            }
            // string[] parts = base64String.Split(',');
            // if (parts.Length >= 2 && parts[0].StartsWith("data:") && parts[1].Contains("/"))
            // {
            //     string mimeType = parts[0].Split(';')[0].Split(":")[1].Trim();
            //     return mimeType;
            // }
            return null;
        }
        public static string? GetMimeType(string base64String)
        {
            string[] parts = base64String.Split(',');
            if (parts.Length >= 2 && parts[0].StartsWith("data:") && parts[1].Contains("/"))
            {
                string mimeType = parts[0].Split(';')[0].Split(":")[1].Trim();
                return mimeType;
            }
            return GetExtensionFromByteSequence(base64String);

        }
        private static bool ByteArrayStartsWith(byte[] byteArray, byte[] magicBytes)
        {
            if (byteArray.Length < magicBytes.Length)
            {
                return false;
            }

            for (int i = 0; i < magicBytes.Length; i++)
            {
                if (byteArray[i] != magicBytes[i])
                {
                    return false;
                }
            }

            return true;
        }

    public string DecryptFile(string encryptedText, string decryptionKey = "OCRAOCRAOCRAOCRA")
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        byte[] keyBytes = Encoding.UTF8.GetBytes(decryptionKey);

        using (AesManaged aes = new AesManaged())
        {
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aes.CreateDecryptor();

            using (MemoryStream ms = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }

    }
}