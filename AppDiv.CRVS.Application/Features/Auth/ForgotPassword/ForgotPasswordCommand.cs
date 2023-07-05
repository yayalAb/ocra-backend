﻿
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Utility.Config;
using AppDiv.CRVS.Utility.Services;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace AppDiv.CRVS.Application.Features.Auth.ForgotPassword
{
    public record ForgotPasswordCommand : IRequest<object>
    {
        public string UserName { get; init; }
        public string ClientURI { get; init; }

    }
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, object>
    {
        private readonly IIdentityService _identityService;
        private readonly IMailService _mailService;
        private readonly ISmsService _smsService;
        private readonly IOptions<SMTPServerConfiguration> config;
        private readonly SMTPServerConfiguration _config;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;
        private readonly HelperService _helperService;

        public ForgotPasswordCommandHandler(IIdentityService identityService, IMailService mailService,
            ISmsService smsService, IOptions<SMTPServerConfiguration> config,
            ILogger<ForgotPasswordCommandHandler> logger,
            HelperService helperService
            )
        {
            _identityService = identityService;
            _mailService = mailService;
            _smsService = smsService;
            this.config = config;
            _config = config.Value;
            _logger = logger;
            _helperService = helperService;
        }
        public async Task<object> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {

                await sendOTP(request, cancellationToken);
                return new { message = "successfully sent password reset by email and phone" };


            }
            catch (Exception)
            {
                throw;
            }


        }
        // private async Task<bool> sendByEmailAsync(ForgotPasswordCommand request, CancellationToken cancellationToken)
        // {
        //     // var response = await _identityService.ForgotPassword(request.Email);
        //     // if (!response.result.Succeeded)
        //     // {
        //     //     throw new Exception(response.result.Errors.ToString());
        //     // }
        //     // var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(response.resetToken)); 
        //     // var param = new Dictionary<string, string?>
        //     // {
        //     //     { "token" , token },
        //     //     { "email" , request.Email }
        //     // };

        //     // var callback = QueryHelpers.AddQueryString(request.ClientURI, param);
        //     // var emailContent = "Please use the link below to reset your password\n" + callback;
        //     // var subject = "Reset Password";
        //     // await _mailService.SendAsync(body: emailContent, subject: subject, senderMailAddress: _config.SENDER_ADDRESS, receiver: request.Email, cancellationToken);
        //     // return  true;


        // }
        private async Task<bool> sendOTP(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {

            var user = await _identityService.GetUserByName(request.UserName);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }
            int expirySecond = 120;
            //send sms and get otp code
            var policySetting = _helperService.getPasswordPolicySetting();
            int codeLength = 6;
            int codeType = 0;
            if (policySetting != null)
            {
                codeLength = policySetting.Max;
                codeType = policySetting.Number && !(policySetting.LowerCase || policySetting.UpperCase || policySetting.OtherChar)
                            ? 0
                            : (policySetting.LowerCase || policySetting.UpperCase || policySetting.OtherChar) && !policySetting.Number
                            ? 1
                            : 2;
            }

            var otpCode = await _smsService.SendOtpAsync(user.PhoneNumber, "", "is your password reset code ", expirySecond, codeLength, codeType);
            if (otpCode == null)
            {
                otpCode = _identityService.GeneratePassword();
            }
            var updateResponse = await _identityService.UpdateResetOtp(user.Id, otpCode?.ToString(), DateTime.Now.AddSeconds(expirySecond));
            if (!updateResponse.Succeeded)
            {
                throw new Exception(string.Join(",", updateResponse.Errors));
            }
            //send to email
            var param = new Dictionary<string, string?>
            {
                { "otp" , otpCode?.ToString() },
                { "userName" , request.UserName }
            };

            var callback = QueryHelpers.AddQueryString(request.ClientURI, param);
            var emailContent = "Please use the link below to reset your password\n" + callback;
            var subject = "Reset Password";
            await _mailService.SendAsync(body: emailContent, subject: subject, senderMailAddress: _config.SENDER_ADDRESS, receiver: user.Email, cancellationToken);




            return true;

        }
    }
}
