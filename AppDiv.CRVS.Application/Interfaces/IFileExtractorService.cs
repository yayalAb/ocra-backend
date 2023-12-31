using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IFileExtractorService
    {
        Task<string[]> ExtractFile(IFormFile file);
    }
}