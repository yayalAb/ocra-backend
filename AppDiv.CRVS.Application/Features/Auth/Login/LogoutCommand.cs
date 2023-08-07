using AppDiv.CRVS.Application.Exceptions;
using MediatR;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using AppDiv.CRVS.Application.Common;
using Microsoft.Extensions.Primitives;

namespace AppDiv.CRVS.Application.Features.Auth.Login

{
    public class LogoutCommand : IRequest<BaseResponse>
    {
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, BaseResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginHistoryRepository _loginHistoryRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserResolverService _userResolverService;
        private readonly HttpClient _httpClient;
        private readonly IRevocationTokenRepository _tokenRepository;


        public LogoutCommandHandler(IRevocationTokenRepository tokenRepository, HttpClient httpClient, IUserResolverService userResolverService, IHttpContextAccessor httpContext, ILoginHistoryRepository loginHistoryRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _loginHistoryRepository = loginHistoryRepository;
            _httpContext = httpContext;
            _userResolverService = userResolverService;
            _httpClient = httpClient;
            _tokenRepository = tokenRepository;

        }
        public async Task<BaseResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            _httpContext.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValue);
            Guid UserId = _userResolverService.GetUserPersonalId();
            var res = new BaseResponse();
            if (UserId == null && UserId == Guid.Empty)
            {
                throw new NotFoundException("User Not Found");
            }
            var response = _userRepository.GetAll().Where(x => x.PersonalInfoId == UserId).FirstOrDefault();
            var tokenLogout = new RevocationToken
            {
                Id = Guid.NewGuid(),
                Token = headerValue.FirstOrDefault(),
                ExpirationDate = DateTime.Now.AddMonths(3)

            };

            var LoginHis = new LoginHistory
            {
                Id = Guid.NewGuid(),
                UserId = response.Id,
                EventType = "Logout",
                EventDate = DateTime.Now,
                IpAddress = _httpContext?.HttpContext?.Connection?.RemoteIpAddress.ToString(),
                Device = _httpContext?.HttpContext?.Request

            };
            await _tokenRepository.InsertAsync(tokenLogout, cancellationToken);
            await _loginHistoryRepository.InsertAsync(LoginHis, cancellationToken);
            await _loginHistoryRepository.SaveChangesAsync(cancellationToken);
            res = new BaseResponse
            {
                Success = false,
                Message = "Logout successfully"
            };
            return res;
        }
    }
}
