
using AppDiv.CRVS.Application.Interfaces;
// using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Infrastructure.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IHttpContextAccessor httpContext;

        public FileUploadService(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext;
        }
        public bool UploadFormFile(IFormFile file , string fileName , string pathToSave, FileMode fileMode = FileMode.Create){
        

            try
            {
                if (file.Length > 0)
                {
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
         public bool UploadBase64File(string base64File , string fileName , string pathToSave, FileMode fileMode = FileMode.Create){
        

            try
            {
               
                return true;
                
            }
            catch (System.Exception)
            {
                
                throw;
            }
            
        }

     
    }
}
