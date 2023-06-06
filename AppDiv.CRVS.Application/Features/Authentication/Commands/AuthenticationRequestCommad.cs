using System;
using System.Collections.Generic;
using System.Linq;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.Authentication.Commands
{
    public class AuthenticationRequestCommad : IRequest<BaseResponse>
    {
        public Guid CertificateId { get; set; }
        public Guid CivilRegOfficer { get; set; }
    }
    public class AuthenticationRequestCommadHandler : IRequestHandler<AuthenticationRequestCommad, BaseResponse>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;
        private readonly IWorkflowService _WorkflowService;
        public AuthenticationRequestCommadHandler(IAuthenticationRepository AuthenticationRepository, IWorkflowService WorkflowService)
        {
            _AuthenticationRepository = AuthenticationRepository;
            _WorkflowService = WorkflowService;
        }
        public async Task<BaseResponse> Handle(AuthenticationRequestCommad request, CancellationToken cancellationToken)
        {

            var AuthenticationRequest = new AuthenticationRequest
            {
                CertificateId = request.CertificateId,
                Request = new Request
                {
                    RequestType = "authentication",
                    CivilRegOfficerId = request.CivilRegOfficer,
                    currentStep = 0
                }
            };
            await _AuthenticationRepository.InsertAsync(AuthenticationRequest, cancellationToken);
            await _AuthenticationRepository.SaveChangesAsync(cancellationToken);
            return new BaseResponse
            {
                Message = "authentication request Sent"
            };
        }
    }
}