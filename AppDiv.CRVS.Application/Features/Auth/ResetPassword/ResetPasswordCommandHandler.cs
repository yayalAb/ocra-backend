
using System.Text;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;

namespace AppDiv.CRVS.Application.Features.Auth.ResetPassword
{

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, BaseResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ISettingRepository _settingRepository;

        public ResetPasswordCommandHandler(IIdentityService identityService, ISettingRepository settingRepository)
        {
            _identityService = identityService;
            _settingRepository = settingRepository;
        }
        public async Task<BaseResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var resetPasswordResponse = new BaseResponse();

            var validator = new ResetPasswordCommandValidator(_settingRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                resetPasswordResponse.Success = false;
                resetPasswordResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    resetPasswordResponse.ValidationErrors.Add(error.ErrorMessage);
                resetPasswordResponse.Message = resetPasswordResponse.ValidationErrors[0];
                resetPasswordResponse.Status = 400;
            }
            if (resetPasswordResponse.Success)
            {
                var user = await _identityService.GetUserByName(request.resetPassword.UserName);
                if (user == null)
                {
                    throw new NotFoundException("user not found");
                }

                if (user.OtpExpiredDate != null && DateTime.Compare((DateTime)user.OtpExpiredDate, DateTime.Now) < 0 || user.Otp != request.resetPassword.Otp)
                {
                    throw new AuthenticationException("could not authenticate user:"+ user.Otp != request.resetPassword.Otp?"\n otp expired":"");
                }
                var forgotPasswordRes = await _identityService.ForgotPassword(email: null, user.UserName);
                if (!forgotPasswordRes.result.Succeeded)
                {
                    throw new Exception(forgotPasswordRes.result.Errors.ToString());
                }
                var resetPasswordRes = await _identityService.ResetPassword(email: null, user.UserName, request.resetPassword.Password, forgotPasswordRes.resetToken);
                if (!resetPasswordRes.Succeeded)
                {
                    throw new Exception();
                }
                resetPasswordResponse.Message = "password reset successful";
                resetPasswordResponse.Status = 200;
            }
            return resetPasswordResponse;

        }
    }
}
