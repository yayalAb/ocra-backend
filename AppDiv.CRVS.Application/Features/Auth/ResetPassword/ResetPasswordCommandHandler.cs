
using System.Text;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;

namespace AppDiv.CRVS.Application.Features.Auth.ResetPassword
{

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, string>
    {
        private readonly IIdentityService _identityService;
        public ResetPasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<string> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.resetPassword.Token));
            var response = await _identityService.ResetPassword(request.resetPassword.Email, request.resetPassword.Email, code);
            if (!response.Succeeded)
            {
                throw new Exception();
            }
            return "password reset successful";
        }
    }
}
