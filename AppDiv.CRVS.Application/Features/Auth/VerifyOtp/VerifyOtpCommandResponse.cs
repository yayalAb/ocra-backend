using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.Auth.VerifyOtp
{
    public class VerifyOtpCommandResponse : BaseResponse
    {
        public string? UserId {get; set;}
        public IList<string>? Roles {get;set;}
        public VerifyOtpCommandResponse() : base()
        {

        }
        //  public CustomerResponseDTO Customer { get; set; }  
    }
}