using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class ResetPasswordRequest
    {
         public string Email { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
        public string? Otp { get; set; }
    }
}


