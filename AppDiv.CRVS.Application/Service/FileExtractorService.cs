using Internal;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
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
            {"image/vnd.djvu", "djvu"},
            {"application/msword", "doc"},
            {"application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx"},
            {"application/vnd.openxmlformats-officedocument.wordprocessingml.template", "dotx"},
            {"application/vnd.ms-word.document.macroEnabled.12", "docm"},
            {"application/vnd.ms-word.template.macroEnabled.12", "dotm"},
            {"image/gif", "gif"},
            {"image/ief", "ief"},
            {"model/iges", "iges"},
            {"model/iges", "igs"},
            {"image/jp2", "jp2"},
            {"image/jpeg", "jpe"},
            {"image/jpeg", "jpeg"},
            {"image/jpeg", "jpg"},
            {"image/png", "png"},
            {"image/svg+xml", "svg"},
            {"image/tiff", "tiff"},
            {"image/tiff", "tif"},
            {"image/vnd.wap.wbmp", "wbmp"},
            {"application/pdf", "pdf"},
            {"image/pict", "pct"},
            {"image/pict", "pic"},
            {"image/png", "png"},
            {"image/x-portable-anymap", "pnm"},
            {"image/x-macpaint", "pnt"},
            {"image/x-macpaint", "pntg"},
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
            {"image/x-quicktime", "qtif"},
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
                        string content = Decrypt(contents, "OCRAOCRAOCRAOCRA");
                        var result = await _eventImportService.ImportEvent(content);

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

                byte[] byteArray = Convert.FromBase64String(base64String);

                // Check if the byte array is not empty
                if (byteArray == null || byteArray.Length == 0)
                {
                    throw new ArgumentException("Byte array is empty or null.");
                }

                // Get the file extension based on the base64 metadata
                return GetExtensionFromBase64Metadata(base64String);
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
        private static string? GetExtensionFromBase64Metadata(string base64String)
        {
            string[] parts = base64String.Split(',');
            if (parts.Length >= 2 && parts[0].StartsWith("data:") && parts[1].Contains("/"))
            {
                string mimeType = parts[1].Split(';')[0];
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
    }
}