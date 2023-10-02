using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using MediatR;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Contracts;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Utility.Services;
using Microsoft.Extensions.Options;
using AppDiv.CRVS.Utility.Config;

namespace AppDiv.CRVS.Application.Features.Auth.VerifyOtp

{
    public class ResendOtpCommand : IRequest<BaseResponse>
    {
        public string UserName { get; set; }
    }

    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, BaseResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IMailService _mailService;
        private readonly ISmsService _smsService;
        private readonly SMTPServerConfiguration _config;
        private readonly HelperService _helperService;

        public ResendOtpCommandHandler(IIdentityService identityService,
                                       IMailService mailService,
                                       IOptions<SMTPServerConfiguration> config,
                                       ISmsService smsService,
                                       HelperService helperService)
        {
            _identityService = identityService;
            _mailService = mailService;
            _smsService = smsService;
            _config = config.Value;
            _helperService = helperService;
        }

        public async Task<BaseResponse> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {

            var user = await _identityService.GetUserByName(request.UserName) ?? throw new NotFoundException($"user with the name {request.UserName} is not found");
            var newOtp = HelperService.GeneratePassword();
            var newOtpExpiredDate = DateTime.Now.AddDays(_helperService.getOtpExpiryDurationSetting());
            var (result, email, phone) = await _identityService.ReGenerateOtp(user.Id, newOtp, newOtpExpiredDate);
            if (!result.Succeeded)
            {
                throw new AuthenticationException(string.Join(",", result.Errors));
            }
            //send otp by email    
            var content = newOtp + "is your new otp code";
            var subject = "OCRVS";
            await _mailService.SendAsync(body: content, subject: subject, senderMailAddress: _config.SENDER_ADDRESS, receiver: email, cancellationToken);

            //send otp by phone 
            await _smsService.SendSMS(phone, subject + "\n" + content);
            return new BaseResponse{Message ="otp resend successfully"};
        }
    }
}