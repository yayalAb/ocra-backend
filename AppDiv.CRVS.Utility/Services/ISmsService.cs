

namespace AppDiv.CRVS.Utility.Services
{
    public interface ISmsService
    {
        //   public bool SendSms(string  to , string body );
        public Task<string> SendOtpAsync(string to, string prefix, string postfix, int expiration, int codeLength, int codeType);
        public Task SendSMS(string to, string message);
        public Task SendBulkSMS(List<string> tos, string message);
    }
}
