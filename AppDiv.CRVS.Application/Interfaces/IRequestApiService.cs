using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IRequestApiService
    {
        public Task Get(string url);
        public Task<string> post(string url, FingerPrintApiRequestDto content);

    }
}