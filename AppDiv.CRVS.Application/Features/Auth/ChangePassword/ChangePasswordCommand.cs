
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Auth.ChangePassword
{
    public record ChangePasswordCommand : IRequest<BaseResponse>
    {
        public string Email { get; init; }
        public string OldPassword { get; init; }
        public string NewPassword { get; init; }

    }
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, BaseResponse>
    {
        private readonly IIdentityService _identityService;

        public ChangePasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<BaseResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var response = await _identityService.ChangePassword(request.Email, request.OldPassword, request.NewPassword);
            if (!response.Succeeded)
            {
                throw new BadRequestException($"could not change password {string.Join(",",response.Errors)}");
            }
            return new BaseResponse{Message = "password changed successfully"};
        }
    }
}
