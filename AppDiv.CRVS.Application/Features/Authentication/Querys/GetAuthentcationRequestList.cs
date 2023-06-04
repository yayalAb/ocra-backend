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

namespace AppDiv.CRVS.Application.Features.Authentication.Querys
{
    public class GetAuthentcationRequestList : IRequest<List<AuthenticationRequestListDTO>>
    {

    }
    public class GetAuthentcationRequestListHandler : IRequestHandler<GetAuthentcationRequestList, List<AuthenticationRequestListDTO>>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;
        private readonly IWorkflowService _WorkflowService;
        public GetAuthentcationRequestListHandler(IAuthenticationRepository AuthenticationRepository, IWorkflowService WorkflowService)
        {
            _AuthenticationRepository = AuthenticationRepository;
            _WorkflowService = WorkflowService;
        }
        public async Task<List<AuthenticationRequestListDTO>> Handle(GetAuthentcationRequestList request, CancellationToken cancellationToken)
        {
            var AuthenticationRequestList = _AuthenticationRepository.GetAll()
            .Include(x => x.Certificate).ThenInclude(x => x.Event)
            .Select(x => new AuthenticationRequestListDTO
            {
                Id = x.Id,
                CertificateId = "certId",
                RequestType = x.Request.RequestType,
                CertificateType = "CertType",
                RequestDate = x.CreatedAt

            });
            if (AuthenticationRequestList == null)
            {
                throw new Exception("Authentication Request not Exist");
            }
            return AuthenticationRequestList.ToList();
        }
    }
}