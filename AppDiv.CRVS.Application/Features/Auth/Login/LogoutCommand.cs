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

namespace AppDiv.CRVS.Application.Features.Auth.Login

{
    public class LogoutCommand : IRequest<BaseResponse>
    {
        public string UserName { get; set; }
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, BaseResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginHistoryRepository _loginHistoryRepository;
        private readonly IHttpContextAccessor _httpContext;

        public LogoutCommandHandler(IHttpContextAccessor httpContext, ILoginHistoryRepository loginHistoryRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _loginHistoryRepository = loginHistoryRepository;
            _httpContext = httpContext;
        }

        public async Task<BaseResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {

            var response = _userRepository.GetAll().Where(x => x.UserName == request.UserName).FirstOrDefault();

            var LoginHis = new LoginHistory
            {
                Id = Guid.NewGuid(),
                UserId = response.Id,
                EventType = "Logout",
                EventDate = DateTime.Now,
                IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                Device = _httpContext.HttpContext.Request.Headers["User-Agent"].ToString()

            };
            await _loginHistoryRepository.InsertAsync(LoginHis, cancellationToken);
            await _loginHistoryRepository.SaveChangesAsync(cancellationToken);
            var res = new BaseResponse
            {
                Success = false,
                Message = "Logout successfully"
            };

            return res;
        }
    }
}