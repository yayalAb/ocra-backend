using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class RequestApiService : IRequestApiService
    {
        private static string Baseurl = "https://bioplugin.cloudabis.com/api/Biometrics/";
        private static string clientKey = "37B68F41C89746CB82587503C381B62C";
        private static string authorizationToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjM3QjY4RjQxQzg5NzQ2Q0I4MjU4NzUwM0MzODFCNjJBIiwiZ2l2ZW5fbmFtZSI6IkZpbmdlcnByaW50IENsb3VkIFNlcnZpY2UiLCJlbWFpbCI6IlkuQWJheW5laEBBcHBkaXYuQ29tIiwicm9sZSI6IkdlbmVyYWxDdXN0b21lciIsIm5iZiI6MTY5MDE5Njc1OCwiZXhwIjoxNjkwMjE4MzU4LCJpYXQiOjE2OTAxOTY3NTh9.W4YJ0DRmHcJVS26XaMRCwpNkSV-qmkw7Dqjas_bZFBE";

        public async Task Get(string url)
        {

        }
        public async Task<string> post(string url, object request)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authorizationToken);
                var jsonData = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Baseurl + url, content);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
    }
}