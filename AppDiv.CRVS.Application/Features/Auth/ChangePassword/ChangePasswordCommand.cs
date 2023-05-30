
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Auth.ChangePassword
{
    public record ChangePasswordCommand : IRequest<BaseResponse>
    {
        public string UserName { get; init; }
        public string OldPassword { get; init; }
        public string NewPassword { get; init; }

    }
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, BaseResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ISettingRepository _settingRepository;

        public ChangePasswordCommandHandler(IIdentityService identityService, ISettingRepository settingRepository)
        {
            _identityService = identityService;
            _settingRepository = settingRepository;
        }
        public async Task<BaseResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
                var changePasswordResponse = new BaseResponse();

                var validator = new ChangePasswordCommandValidator(_settingRepository);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                //Check and log validation errors
                if (validationResult.Errors.Count > 0)
                {
                    changePasswordResponse.Success = false;
                    changePasswordResponse.ValidationErrors = new List<string>();
                    foreach (var error in validationResult.Errors)
                        changePasswordResponse.ValidationErrors.Add(error.ErrorMessage);
                    changePasswordResponse.Message = changePasswordResponse.ValidationErrors[0];
                    changePasswordResponse.Status = 400;
                }
                if (changePasswordResponse.Success)
                {
                    var response = await _identityService.ChangePassword(request.UserName, request.OldPassword, request.NewPassword);
                    if (!response.Succeeded)
                    {
                        throw new BadRequestException($"could not change password {string.Join(",", response.Errors)}");
                    }
                    changePasswordResponse.Message =  "password changed successfully";
                    changePasswordResponse.Status = 200;
                }
                return changePasswordResponse;
            



        }
    }
}
