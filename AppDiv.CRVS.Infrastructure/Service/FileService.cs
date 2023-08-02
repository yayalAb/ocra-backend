
using System;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;

using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Formats;

namespace AppDiv.CRVS.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
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

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }
        public bool UploadFormFile(IFormFile file, string fileName, string pathToSave, FileMode fileMode = FileMode.Create)
        {


            try
            {
                if (file.Length > 0)
                {
                    if (!Directory.Exists(pathToSave))
                    {
                        // If folder does not exist, create it
                        Directory.CreateDirectory(pathToSave);
                    }
                    var matchingFiles = Directory.GetFiles(pathToSave, fileName + "*");
                    //removing file with the same id but different extension 
                    matchingFiles.ToList().ForEach(file =>
                    {
                        System.IO.File.Delete(file);
                    });
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(fullPath, fileMode))
                    {
                        file.CopyTo(stream);
                    }
                }
                return true;

            }
            catch (System.Exception)
            {

                throw;
            }

        }
        public async Task<bool> UploadBase64FileAsync(string base64String, string fileName, string pathToSave, FileMode? fileMode = FileMode.Create)
        {


            if (!Directory.Exists(pathToSave))
            {
                // If folder does not exist, create it
                Directory.CreateDirectory(pathToSave);
            }


            if (!HelperService.IsBase64String(base64String))
            {
                return false;

            }
            // Convert the Base64 string to a byte array.
            string myString = base64String.Substring(base64String.IndexOf(',') + 1);
            byte[] bytes = Convert.FromBase64String(myString);

            var extension = GetFileExtensionFromBase64String(base64String) ?? ".bin";
            var fullPath = Path.Combine(pathToSave, fileName + extension);
            await File.WriteAllBytesAsync(fullPath, bytes);

            return true;


        }

        public async Task<bool> UploadBase64FilesAsync(IList<string> base64Strings, IList<Guid> fileNames, string pathToSave, FileMode? fileMode = FileMode.Create)
        {


            try
            {
                //    _logger.LogCritical(myString);
                var count = 0;
                foreach (var file in base64Strings)
                {
                    // exclude unwanted characters
                    // string myString = file.Substring(file.IndexOf(',') + 1);
                    // // Convert the Base64 string to a byte array.
                    // byte[] bytes = Convert.FromBase64String(file);
                    // var extension = string.IsNullOrEmpty(getFileExtension(bytes)) ? "." + getFileExtension(bytes) : ".png";
                    // var fullPath = Path.Combine(pathToSave, $"{fileNames[count++]}{extension}");
                    // await File.WriteAllBytesAsync(fullPath, bytes);
                    await UploadBase64FileAsync(file, fileNames[count++].ToString(), pathToSave, FileMode.Create);

                }
                return true;

            }
            catch (System.Exception)
            {

                throw;
            }

        }
        public (byte[] file, string fileName, string fileExtenion) getFile(string fileId, string folder, string? eventType)
        {
            try
            {
                string folderName;
                if (eventType != null)
                {
                    folderName = Path.Combine("Resources", folder, eventType);
                }
                else
                {

                    folderName = Path.Combine("Resources", folder);
                }
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                var matchingFiles = Directory.GetFiles(fullPath, fileId + "*");

                if (matchingFiles.Length == 0)
                {
                    throw new NotFoundException("file not found");

                }
                var actualFilePath = matchingFiles.First();
                var fileExtension = Path.GetExtension(actualFilePath);
                var actualFileName = Path.GetFileNameWithoutExtension(actualFilePath);

                _logger.LogCritical(fileExtension);

                var fileContent = System.IO.File.ReadAllBytes(actualFilePath);
                ;
                return (file: fileContent, fileName: actualFileName, fileExtenion: fileExtension);

            }
            catch (System.IO.DirectoryNotFoundException e)
            {

                throw new BadRequestException($"could not find the directory of the path specified:\n{e.Message}");
            }

        }

        private string? getImageFileExtension(byte[] bytes)
        {

            try
            {
                // Use ImageSharp to identify the image format
                IImageFormat format = Image.DetectFormat(bytes);
                // Get the file extension from the image format
                return format?.FileExtensions.FirstOrDefault();

            }
            catch (Exception e)
            {
                return null;
            }

        }
        public static string? GetFileExtensionFromBase64String(string base64String)
        {
            // Convert the base64 string to a byte array
            byte[] byteArray = Convert.FromBase64String(base64String);

            // Check if the byte array is not empty
            if (byteArray == null || byteArray.Length == 0)
            {
                throw new ArgumentException("Byte array is empty or null.");
            }

            // Get the file extension based on the base64 metadata
            return GetExtensionFromBase64Metadata(base64String);
        }

        private static string? GetExtensionFromBase64Metadata(string base64String)
        {
            // The base64 string is typically in the format "data:image/png;base64,iVBORw0KGgoAAA..."
            // We can attempt to extract the file extension from the metadata part "image/png"
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
