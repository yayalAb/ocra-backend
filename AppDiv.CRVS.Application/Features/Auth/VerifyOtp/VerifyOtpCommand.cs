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

namespace AppDiv.CRVS.Application.Features.Auth.VerifyOtp

{
    public class VerifyOtpCommand : IRequest<BaseResponse>
    {
        public string UserName { get; set; }
        public string Otp {get; set; }
    }

    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, BaseResponse>
    {
        private readonly IIdentityService _identityService;

        public VerifyOtpCommandHandler( IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<BaseResponse> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var res = await _identityService.VerifyOtp(request.UserName, request.Otp);
            
            return new BaseResponse();
        }
    }
}