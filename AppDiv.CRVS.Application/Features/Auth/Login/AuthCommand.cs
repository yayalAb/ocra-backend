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
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Utility.Config;
using AppDiv.CRVS.Domain.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AppDiv.CRVS.Application.Features.Auth.Login

{
    public class AuthCommand : IRequest<AuthResponseDTO>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class AuthCommandHandler : IRequestHandler<AuthCommand, AuthResponseDTO>
    {
        private readonly ITokenGeneratorService _tokenGenerator;
        private readonly ILogger<AuthCommandHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly HelperService _helperService;
        private readonly IMailService _mailService;
        private readonly ISmsService _smsService;
        private readonly IDateAndAddressService _addressService;
        private readonly IIdentityService _identityService;
        private readonly ILoginHistoryRepository _loginHistoryRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SMTPServerConfiguration _config;

        public AuthCommandHandler(IHttpContextAccessor httpContext,
                                  ILoginHistoryRepository loginHistoryRepository,
                                  IIdentityService identityService,
                                  ITokenGeneratorService tokenGenerator,
                                  ILogger<AuthCommandHandler> logger,
                                  IUserRepository userRepository,
                                  HelperService helperService,
                                  IMailService mailService,
                                  IOptions<SMTPServerConfiguration> config,
                                  ISmsService smsService,
                                  IDateAndAddressService addressService)
        {
            _identityService = identityService;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
            _userRepository = userRepository;
            _helperService = helperService;
            _mailService = mailService;
            _smsService = smsService;
            this._addressService = addressService;
            _loginHistoryRepository = loginHistoryRepository;
            _httpContext = httpContext;
            _config = config.Value;
        }

        public async Task<AuthResponseDTO> Handle(AuthCommand request, CancellationToken cancellationToken)
        {

            var response = await _identityService.AuthenticateUser(request.UserName, request.Password);
            //if locked or unauthenticated or diactivated
            if (!response.result.Succeeded)
            {
                throw new AuthenticationException(string.Join(",", response.result.Errors));
            }
            if (response.status == AuthStatus.FirstTimeLogin || response.status == AuthStatus.OtpUnverified)
            {
                return new AuthResponseDTO
                {
                    UserId = response.userId,
                    Name = request.UserName,
                    isFirstTime = response.status == AuthStatus.FirstTimeLogin,
                    isOtpUnverified = response.status == AuthStatus.OtpUnverified
                };

            }
            //\\\\\\\\\\\

            // otp expired , generate new otp 
            if (response.status == AuthStatus.OtpExpired)
            {
                var newOtp = _identityService.GeneratePassword();
                var newOtpExpiredDate = DateTime.Now.AddDays(_helperService.getOtpExpiryDurationSetting());
                var res = await _identityService.ReGenerateOtp(response.userId, newOtp, newOtpExpiredDate);
                if (!res.result.Succeeded)
                {
                    throw new AuthenticationException(string.Join(",", response.result.Errors));
                }
                //send otp by email    
                var content = newOtp + "is your new otp code";
                var subject = "OCRVS";
                await _mailService.SendAsync(body: content, subject: subject, senderMailAddress: _config.SENDER_ADDRESS, receiver: res.email, cancellationToken);

                //send otp by phone 
                await _smsService.SendSMS(res.phone, subject + "\n" + content);
                return new AuthResponseDTO
                {
                    UserId = response.userId,
                    Name = request.UserName,
                    isOtpExpired = true
                };
            }
            ////\\\\\\\\

            //else if active
            var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
                                                {
                                                    { "UserGroups", NavigationPropertyType.COLLECTION },
                                                    { "Address", NavigationPropertyType.REFERENCE },
                                                    { "PersonalInfo", NavigationPropertyType.REFERENCE }

                                                };
            var userData = await _userRepository.GetWithAsync(response.userId, explicitLoadedProperties);


            string token = _tokenGenerator.GenerateJWTToken((userData.Id, userData.UserName, userData.PersonalInfoId, response.roles, userData.AddressId, userData.Address.AdminLevel));


            var userRoles = userData.UserGroups.SelectMany(ug => ug.Roles
            .Select(r => new RoleDto
            {
                page = r.Value<string>("page") ?? "",
                title = r.Value<string>("title") ?? "",
                canAdd = r.Value<bool>("canAdd"),
                canDelete = r.Value<bool>("canDelete"),
                canViewDetail = r.Value<bool>("canViewDetail"),
                canView = r.Value<bool>("canView"),
                canUpdate = r.Value<bool>("canUpdate")
            })).GroupBy(r => r.page.Trim(), StringComparer.OrdinalIgnoreCase).Select(g => new RoleDto
            {
                page = g.Key,
                title = g.FirstOrDefault()?.title ?? "",
                canAdd = g.Aggregate(false, (acc, x) => acc || x.canAdd),
                canDelete = g.Aggregate(false, (acc, x) => acc || x.canDelete),
                canUpdate = g.Aggregate(false, (acc, x) => acc || x.canUpdate),
                canView = g.Aggregate(false, (acc, x) => acc || x.canView),
                canViewDetail = g.Aggregate(false, (acc, x) => acc || x.canViewDetail)
            }).ToList();
            if (userData.CanRegisterEvent != null && userData.CanRegisterEvent == true)
            {
                Console.WriteLine($"yyyyyy..........{userData.CanRegisterEvent}");
                string marriage = Enum.GetName<Page>(Page.Marriage)?.ToLower()!;
                string birth = Enum.GetName<Page>(Page.Birht)?.ToLower()!;
                string adoption = Enum.GetName<Page>(Page.Adoption)?.ToLower()!;
                string death = Enum.GetName<Page>(Page.Death)?.ToLower()!;
                string divorce = Enum.GetName<Page>(Page.Devorce)?.ToLower()!;
                string marriageApplication = Enum.GetName<Page>(Page.MarriageApplication)?.ToLower()!;
                string marriageApplicationList = Enum.GetName<Page>(Page.MarriageApplicationList)?.ToLower()!;

                userRoles.Where(p =>
                           p.page.ToLower() == marriage
                           || p.page.ToLower() == birth
                           || p.page.ToLower() == adoption
                           || p.page.ToLower() == death
                           || p.page.ToLower() == divorce
                           || p.page.ToLower() == marriageApplication
                           || p.page.ToLower() == marriageApplicationList)
                           .ToList().ForEach(r =>
                   {
                       r.canAdd = true;
                       r.canView = true;
                       r.canUpdate = true;
                       r.canViewDetail = true;
                       r.canDelete = true;
                   });
            }
            var LoginHis = new LoginHistory
            {
                Id = Guid.NewGuid(),
                UserId = response.userId,
                EventType = "Login",
                EventDate = DateTime.Now,
                IpAddress = _httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                Device = _httpContext.HttpContext?.Request.Headers["User-Agent"].ToString()

            };
            await _loginHistoryRepository.InsertAsync(LoginHis, cancellationToken);
            await _loginHistoryRepository.SaveChangesAsync(cancellationToken);

            return new AuthResponseDTO()
            {
                UserId = userData.Id,
                Name = userData.UserName,
                AdminLevel = userData?.Address?.AdminLevel,
                AddressId = userData?.AddressId,
                AddressCode = userData?.Address?.Code,
                Token = token,
                PreferedLanguage = userData?.PreferedLanguage,
                PersonalInfoId = userData.PersonalInfoId,
                GroupIds = userData.UserGroups.Select(g => g.Id).ToList(),
                Roles = userRoles.ToList(),
                FirstName = userData.PersonalInfo?.FirstName,
                MiddleName = userData.PersonalInfo?.MiddleName,
                LastName = userData.PersonalInfo?.LastName,
                CanRegisterEvent = userData.CanRegisterEvent,
                Address = await _addressService.FormatedAddress(userData.AddressId)!
            };
        }
    }
}