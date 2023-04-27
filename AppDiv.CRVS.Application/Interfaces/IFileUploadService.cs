using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IFileService
    {
       public bool UploadFormFile(IFormFile file, string fileName, string pathToSave , FileMode fileMode);
        public bool UploadBase64File(string base64String, string fileName, string pathToSave, FileMode? fileMode);
    }
}
