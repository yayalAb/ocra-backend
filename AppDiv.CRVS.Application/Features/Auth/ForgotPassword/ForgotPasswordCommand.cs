
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Utility.Config;
using AppDiv.CRVS.Utility.Services;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace AppDiv.CRVS.Application.Features.Auth.ForgotPassword
{
    public record ForgotPasswordCommand : IRequest<string>
    {
        public string Email { get; init; }
        public string Type { get; set; }
        public string? ClientURI { get; init; }
    }
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
    {
        private readonly IIdentityService _identityService;
        private readonly IMailService _mailService;
        private readonly ISmsService _smsService;
        private readonly IOptions<SMTPServerConfiguration> config;
        private readonly SMTPServerConfiguration _config;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(IIdentityService identityService, IMailService mailService, ISmsService smsService, IOptions<SMTPServerConfiguration> config, ILogger<ForgotPasswordCommandHandler> logger)
        {
            _identityService = identityService;
            _mailService = mailService;
            _smsService = smsService;
            this.config = config;
            _config = config.Value;
            _logger = logger;
        }
        public async Task<string> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                switch (request.Type.ToUpper())
                {
                    case "EMAIL":
                        return await sendByEmailAsync(request, cancellationToken);
                    case "OTP":
                        return await sendOTP(request, cancellationToken);
                    default:
                        throw new BadRequestException("Invalid Type :type must be either email or otp");
                }
            }
            catch (Exception)
            {
                throw;
            }


        }
        private async Task<string> sendByEmailAsync(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var response = await _identityService.ForgotPassword(request.Email);
            if (!response.result.Succeeded)
            {
                throw new Exception(response.result.Errors.ToString());
            }
            var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(response.resetToken)); ;
            var param = new Dictionary<string, string?>
            {
                { "token" , token },
                { "email" , request.Email }
            };

            var callback = QueryHelpers.AddQueryString(request.ClientURI, param);
            var emailContent = "Please use the link below to reset your password\n" + callback;
            var subject = "Reset Password";
            await _mailService.SendAsync(body: emailContent, subject: subject, senderMailAddress: _config.SENDER_ADDRESS, receiver: request.Email, cancellationToken);
            return "successfully sent password reset link by email";


        }
        private async Task<string> sendOTP(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }

            Random random = new Random();
            var otpCode = random.Next(000000, 999999);
            var updateResponse = await _identityService.UpdateUser(user.Id, user.UserName, user.Email, user.PersonalInfoId, otpCode.ToString(), DateTime.Now.AddMinutes(2));
            if (!updateResponse.Succeeded)
            {
                throw new Exception(string.Join(",", updateResponse.Errors));
            }
            // _smsService.SendSms(user.PhoneNumber , otpCode.ToString());

            return $"successfully sent password reset OTP";

        }
    }
}
