
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Auth.UnlockUser
{
    public record UnlockUserCommand : IRequest<BaseResponse>
    {
        public string UserName { get; init; }

    }
    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, BaseResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ISettingRepository _settingRepository;

        public UnlockUserCommandHandler(IIdentityService identityService, ISettingRepository settingRepository)
        {
            _identityService = identityService;
            _settingRepository = settingRepository;
        }
        public async Task<BaseResponse> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            var unlockUserResponse = new BaseResponse();


            var response = await _identityService.UnlockUserAsync(request.UserName);
            if (!response.Succeeded)
            {
                throw new BadRequestException($"could not Unlock user {string.Join(",", response.Errors)}");
            }
            unlockUserResponse.Message = "User unlocked successfully";
            unlockUserResponse.Status = 200;

            return unlockUserResponse;




        }
    }
}
