using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Interfaces
{

    public interface IConvertExcelFileToLookupObjectService
    {
        public Task<BaseResponse> ConvertFileToObject(IFormFile ImportedExcel, CancellationToken cancellationToken);

    }

}