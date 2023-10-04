using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Domain;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IContentValidator
    {
        Task<BaseResponse> ValidateAsync(string eventType, JObject content, bool IsUpdate = true);
        Task<BaseResponse> ValidateUserDataAsync(ApplicationUser oldData, JObject content, bool IsUpdate = true);
    }
}