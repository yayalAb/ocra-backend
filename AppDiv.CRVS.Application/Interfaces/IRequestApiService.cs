using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IRequestApiService
    {
        public Task Get(string url);
        public Task<string> post(string url, object content);

    }
}