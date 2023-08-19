using System.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;
using System.Drawing;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class RequestApiService : IRequestApiService
    {
        private static string Baseurl = "https://bioplugin.cloudabis.com/api/Biometrics/";
        public string clientKey; //= "37B68F41C89746CB82587503C381B62C";
        public string authorizationToken;// = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjM3QjY4RjQxQzg5NzQ2Q0I4MjU4NzUwM0MzODFCNjJBIiwiZ2l2ZW5fbmFtZSI6IkZpbmdlcnByaW50IENsb3VkIFNlcnZpY2UiLCJlbWFpbCI6IlkuQWJheW5laEBBcHBkaXYuQ29tIiwicm9sZSI6IkdlbmVyYWxDdXN0b21lciIsIm5iZiI6MTY5MDI2Njg5MSwiZXhwIjoxNjkwMjg4NDkxLCJpYXQiOjE2OTAyNjY4OTF9.DYUCbyHRCkGCotVLcYbkAzT-l8ECKtrkeyoGgKJqc2s";
        private readonly HttpClient _client;
        private readonly IFingerprintApiKeyRepostory _apikeyRepository;
        private readonly ISettingRepository _settingRepository;

        public RequestApiService(IFingerprintApiKeyRepostory apikeyRepository, ISettingRepository settingRepository)
        {
            _client = new HttpClient();
            _apikeyRepository = apikeyRepository;
            _settingRepository = settingRepository;
            var tokens = _settingRepository.GetAll().Where(x => x.Key == "passwordPolicy").FirstOrDefault();
            clientKey = tokens.Value?.Value<string>("clientKey");
            authorizationToken = tokens.Value?.Value<string>("token");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authorizationToken);
        }
        public async Task Get(string url)
        {

        }
        public async Task<string> post(string url, FingerPrintApiRequestDto request)
        {
            request.clientKey = clientKey;
            var jsonData = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(Baseurl + url, content);
            if (response.StatusCode.ToString() == "Unauthorized")
            {
                await refrashToken();
                response = await _client.PostAsync(Baseurl + url, content);
            }
            var res = response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response yyyyyyyyyyyyyyyy : {0}",responseBody);
            return responseBody;
        }

        private async Task<string> refrashToken()
        {
            var defualtAddress = _settingRepository.GetAll().Where(x => x.Key == "passwordPolicy").FirstOrDefault();
            if (defualtAddress == null)
            {
                throw new NotFoundException("Api Key Does not Found");
            }
            string clientAPIKey = defualtAddress.Value?.Value<string>("clientAPIKey");
            string clientKey1 = defualtAddress.Value?.Value<string>("clientKey");
            var request = new
            {
                clientAPIKey = clientAPIKey,
                clientKey = clientKey
            };
            using (var httpClient = new HttpClient())
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var LonginResponse = await httpClient.PostAsync("https://bioplugin.cloudabis.com/api/Authorizations/Token", content);
                LonginResponse.EnsureSuccessStatusCode();
                var responseBody = await LonginResponse.Content.ReadAsStringAsync();
                FingerprintApiLoginResponseDto responsData = JsonSerializer.Deserialize<FingerprintApiLoginResponseDto>(responseBody);
                if (responsData?.responseData?.accessToken != null)
                {
                    clientKey = clientKey1;
                    authorizationToken = responsData.responseData.accessToken;
                }
                UpdateTokenDto passwordpo = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateTokenDto>(defualtAddress.Value.ToString());
                passwordpo.token = authorizationToken;
                string updatedJson = Newtonsoft.Json.JsonConvert.SerializeObject(passwordpo);
                defualtAddress.Value = JObject.Parse(updatedJson);
                await _settingRepository.UpdateAsync(defualtAddress, x => x.Id);
                _settingRepository.SaveChanges();
            }
            return "";
        }

    }
}