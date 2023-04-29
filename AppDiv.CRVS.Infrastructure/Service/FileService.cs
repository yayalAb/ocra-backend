
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
                    var matchingFiles = Directory.GetFiles(pathToSave, fileName + "*");
                    //removing file with the same id but different extension 
                    matchingFiles.ToList().ForEach(file => {
                        //TODO: delete the file
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
        public bool UploadBase64File(string base64String, string fileName, string pathToSave, FileMode? fileMode = FileMode.Create)
        {


            try
            {

                // Convert the Base64 string to a byte array.
                string myString = base64String.Substring(base64String.IndexOf(',') + 1);
                byte[] bytes = Convert.FromBase64String(base64String);
                var extension = string.IsNullOrEmpty(getFileExtension(bytes)) ? "." + getFileExtension(bytes) : ".png";
                var fullPath = Path.Combine(pathToSave, fileName + extension);
                File.WriteAllBytes(fullPath, bytes);
                return true;

            }
            catch (System.Exception)
            {

                throw;
            }

        }
        public (byte[] file, string fileName, string fileExtenion) getFile(string fileId, string folder)
        {

            var folderName = Path.Combine("Resources", folder);
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

        private string? getFileExtension(byte[] bytes)
        {
            // Use ImageSharp to identify the image format
            IImageFormat format = Image.DetectFormat(bytes);

            // Get the file extension from the image format
            return format?.FileExtensions.FirstOrDefault();

        }

    }
}
