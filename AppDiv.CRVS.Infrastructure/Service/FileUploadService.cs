
using AppDiv.CRVS.Application.Interfaces;
// using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IHttpContextAccessor httpContext;

        public FileService(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext;
        }
        public bool UploadFormFile(IFormFile file, string fileName, string pathToSave, FileMode fileMode = FileMode.Create)
        {


            try
            {
                if (file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
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
                byte[] bytes = Convert.FromBase64String(base64String);
                var fullPath = Path.Combine(pathToSave, fileName);
                File.WriteAllBytes(fullPath, bytes);
                return true;

            }
            catch (System.Exception)
            {

                throw;
            }

        }
        public byte[] getFile(Guid fileId, string folder, string extension)
        {
            var folderName = Path.Combine("Resources", folder);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileId.ToString()+extension);
          return   System.IO.File.ReadAllBytes(fullPath);
        }

    }
}
