﻿using AppDiv.CRVS.Application.Exceptions;
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
        private readonly IIdentityService _identityService;
        private readonly ILoginHistoryRepository _loginHistoryRepository;
        private readonly IHttpContextAccessor _httpContext;

        public AuthCommandHandler(IHttpContextAccessor httpContext, ILoginHistoryRepository loginHistoryRepository, IIdentityService identityService, ITokenGeneratorService tokenGenerator, ILogger<AuthCommandHandler> logger, IUserRepository userRepository)
        {
            _identityService = identityService;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
            _userRepository = userRepository;
            _loginHistoryRepository = loginHistoryRepository;
            _httpContext = httpContext;
        }

        public async Task<AuthResponseDTO> Handle(AuthCommand request, CancellationToken cancellationToken)
        {

            var response = await _identityService.AuthenticateUser(request.UserName, request.Password);

            if (!response.result.Succeeded)
            {
                throw new AuthenticationException(string.Join(",", response.result.Errors));
            }
            var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
                                                {
                                                    { "UserGroups", NavigationPropertyType.COLLECTION },

                                                };
            var userData = await _userRepository.GetWithAsync(response.userId, explicitLoadedProperties);
            string token = _tokenGenerator.GenerateJWTToken((userData.Id, userData.UserName, userData.PersonalInfoId, response.roles));
            // _logger.LogCritical("auth");
            // _logger.LogCritical(userData.Id);

            var userRoles = userData.UserGroups.SelectMany(ug => ug.Roles
            .Select(r => new RoleDto
            {
                Page = r.Value<string>("Page") ?? "",
                Title = r.Value<string>("Title") ?? "",
                CanAdd = r.Value<bool>("CanAdd"),
                CanDelete = r.Value<bool>("CanDelete"),
                CanViewDetail = r.Value<bool>("CanViewDetail"),
                CanView = r.Value<bool>("CanView"),
                CanUpdate = r.Value<bool>("CanUpdate")
            })).GroupBy(r => r.Page.Trim(), StringComparer.OrdinalIgnoreCase).Select(g => new RoleDto
            {
                Page = g.Key,
                Title = g.FirstOrDefault()?.Title ?? "",
                CanAdd = g.Aggregate(false, (acc, x) => acc || x.CanAdd),
                CanDelete = g.Aggregate(false, (acc, x) => acc || x.CanDelete),
                CanUpdate = g.Aggregate(false, (acc, x) => acc || x.CanUpdate),
                CanView = g.Aggregate(false, (acc, x) => acc || x.CanView),
                CanViewDetail = g.Aggregate(false, (acc, x) => acc || x.CanViewDetail)
            });
            var LoginHis = new LoginHistory
            {
                Id = Guid.NewGuid(),
                UserId = response.userId,
                EventType = "Login",
                EventDate = DateTime.Now,
                IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                Device = _httpContext.HttpContext.Request.Headers["User-Agent"].ToString()

            };
            await _loginHistoryRepository.InsertAsync(LoginHis, cancellationToken);
            await _loginHistoryRepository.SaveChangesAsync(cancellationToken);
            return new AuthResponseDTO()
            {
                UserId = userData.Id,
                Name = userData.UserName,
                Token = token,
                PersonalInfoId = userData.PersonalInfoId,
                GroupIds = userData.UserGroups.Select(g => g.Id).ToList(),
                Roles = userRoles.ToList(),
            };
        }
    }
}