using MediatR;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.Auth.VerifyOtp

{
    public class VerifyOtpCommand : IRequest<VerifyOtpCommandResponse>
    {
        public string UserName { get; set; }
        public string Otp { get; set; }
    }

    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, VerifyOtpCommandResponse>
    {
        private readonly IIdentityService _identityService;

        public VerifyOtpCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<VerifyOtpCommandResponse> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var res = await _identityService.VerifyOtp(request.UserName, request.Otp);
            var verifyOtpResponse = new VerifyOtpCommandResponse();
            if (res.result.Succeeded)
            {
                verifyOtpResponse.UserId = res.userId;
                verifyOtpResponse.Roles = res.roles;
            }

            return verifyOtpResponse;
        }
    }
}