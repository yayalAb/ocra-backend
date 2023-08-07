
using System;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;

using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Service;
using Hangfire.Annotations;
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

            var extension = FileExtractorService.GetFileExtensionFromBase64String(base64String) ?? ".bin";
            var fullPath = Path.Combine(pathToSave, fileName + extension);
            var matchingFiles = Directory.GetFiles(pathToSave, fileName + "*");
            //removing file with the same id but different extension 
            matchingFiles.ToList().ForEach(file =>
            {
                System.IO.File.Delete(file);
            });

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
        public (byte[] file, string fileName, string fileExtenion) getFile(string fileId, string folder, string? eventType, string? fingerPrintIndex)
        {
            try
            {
                string folderName;
                string fileName = fileId;
                if (eventType != null)
                {

                    folderName = Path.Combine("Resources", folder, eventType);
                }
               else if (!string.IsNullOrEmpty(fingerPrintIndex))
                {
                    folderName = Path.Combine("Resources", folder, fileId);
                    fileName = fingerPrintIndex;
                }
                else
                {

                    folderName = Path.Combine("Resources", folder);
                }
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var matchingFiles = Directory.GetFiles(fullPath, fileName + "*");

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

        public List<string> GetFileNamesInfolder(string folder)
        {
            try
            {

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), folder);
                return Directory.GetFiles(fullPath).ToList();
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }
        public (byte[] file, string fileName, string fileExtenion) getFile(string fullPath)
        {
            try
            {


                var fileExtension = Path.GetExtension(fullPath);
                var actualFileName = Path.GetFileNameWithoutExtension(fullPath);

                _logger.LogCritical(fileExtension);
                var fileContent = System.IO.File.ReadAllBytes(fullPath);
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



    }
}
