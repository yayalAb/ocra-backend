using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Service
{
    public class FileExtractorService : IFileExtractorService
    {

        public string[] ExtractFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new NotFoundException("file not uploaded.");
            }
            string extractPath = Path.Combine(Directory.GetCurrentDirectory(), "ExtractedFiles");
            try
            {
                Directory.CreateDirectory(extractPath);
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

                return extractedFiles;
            }
            catch (Exception ex)
            {
                throw new NotFoundException(ex.Message);
            }
        }
    }
}