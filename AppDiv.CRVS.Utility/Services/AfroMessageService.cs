



using System.Text;
using AppDiv.CRVS.Utility.Config;
using AppDiv.CRVS.Utility.Contracts;
using AppDiv.CRVS.Utility.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class AfroMessageService : ISmsService
{
    private readonly AfroMessageConfiguration _config;

    public AfroMessageService(IOptions<AfroMessageConfiguration> afroMsgConfig)
    {

        _config = afroMsgConfig.Value;
    }
    public async Task<string> SendOtpAsync(string to, string prefix, string postfix, int expiration, int codeLength, int codeType)
    {
        //codeType =  {0 : numberOnly , 1: alphabetOnly , 2:alphanumeric }
        string? otpCode;
        using (var client = new HttpClient())
        {
            var url = $"https://api.afromessage.com/api/challenge?\from={_config.From}&sender={_config.Sender}&to={to}&pr={prefix}&ps={postfix}&sb={1}&sa={1}&ttl={expiration}&len={codeLength}&t={codeType}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {_config.Token}");
            var response = await client.SendAsync(request);
            // Ensure the response was successful
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var decodedData = JsonConvert.DeserializeObject<AfroMsgResponseModel>(responseBody);
            otpCode = decodedData?.response.code;

        }
        return otpCode;
    }
    public async Task SendSMS(string to, string message)
    {
        if (to != null)
        {

            using (var client = new HttpClient{Timeout = TimeSpan.FromSeconds(100)})
            {
                try
                {
                var url = "https://api.afromessage.com/api/send";
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                var jsonContent = new
                {
                    to = to,
                    from = _config.From,
                    sender = _config.Sender,
                    message = message,
                };
                var json = JsonConvert.SerializeObject(jsonContent);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", $"Bearer {_config.Token}");
                var response = await client.SendAsync(request);
                // Ensure the response was successful
                // response.EnsureSuccessStatusCode();
                    
                }
                catch (System.Exception e)
                {
                    
                   Console.WriteLine("afroMessage error : "+ e);
                }

            }

        }

    }
    public async Task SendBulkSMS(List<string> tos, string message)
    {

        foreach (var to in tos)
        {
            await this.SendSMS(to, message);
        }
    }


}
