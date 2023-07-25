using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class RequestApiService : IRequestApiService
    {
        private static string Baseurl = "https://bioplugin.cloudabis.com/api/Biometrics/";
        private static string clientKey; //= "37B68F41C89746CB82587503C381B62C";
        private static string authorizationToken;// = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjM3QjY4RjQxQzg5NzQ2Q0I4MjU4NzUwM0MzODFCNjJBIiwiZ2l2ZW5fbmFtZSI6IkZpbmdlcnByaW50IENsb3VkIFNlcnZpY2UiLCJlbWFpbCI6IlkuQWJheW5laEBBcHBkaXYuQ29tIiwicm9sZSI6IkdlbmVyYWxDdXN0b21lciIsIm5iZiI6MTY5MDI2Njg5MSwiZXhwIjoxNjkwMjg4NDkxLCJpYXQiOjE2OTAyNjY4OTF9.DYUCbyHRCkGCotVLcYbkAzT-l8ECKtrkeyoGgKJqc2s";
        private readonly HttpClient _client;
        private readonly IFingerprintApiKeyRepostory _apikeyRepository;

        public RequestApiService(IFingerprintApiKeyRepostory apikeyRepository)
        {
            _client = new HttpClient();
            _apikeyRepository = apikeyRepository;
            var tokens = _apikeyRepository.GetAll().FirstOrDefault();
            if (tokens == null)
            {
                throw new NotFoundException("Please refrash your token");
            }
            clientKey = tokens.clientKey;
            authorizationToken = tokens.token;

            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authorizationToken);
        }
        public async Task Get(string url)
        {

        }
        public async Task<string> post(string url, object request)
        {
            var jsonData = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(Baseurl + url, content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }



    }
}