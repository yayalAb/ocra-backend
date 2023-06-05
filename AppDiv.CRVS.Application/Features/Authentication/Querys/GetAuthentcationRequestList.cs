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
        public Guid UserId { get; set; }

    }
    public class GetAuthentcationRequestListHandler : IRequestHandler<GetAuthentcationRequestList, List<AuthenticationRequestListDTO>>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IRequestRepostory _RequestRepostory;

        public GetAuthentcationRequestListHandler(IRequestRepostory RequestRepostory, IAuthenticationRepository AuthenticationRepository, IWorkflowService WorkflowService)
        {
            _AuthenticationRepository = AuthenticationRepository;
            _WorkflowService = WorkflowService;
            _RequestRepostory = RequestRepostory;
        }
        public async Task<List<AuthenticationRequestListDTO>> Handle(GetAuthentcationRequestList request, CancellationToken cancellationToken)
        {
            var RequestList = _RequestRepostory.GetAll()
            .Include(x => x.CivilRegOfficer)
            .Include(x => x.AuthenticationRequest)
            .Include(x => x.CorrectionRequest)
            .Select(x => new AuthenticationRequestListDTO
            {
                Id = x.Id,
                RequestedBy = x.CivilRegOfficer.FirstNameLang + " " + x.CivilRegOfficer.MiddleNameLang + " " + x.CivilRegOfficer.LastNameLang,
                RequestType = x.RequestType,
                CurrentStep = x.currentStep,
                RequestDate = x.CreatedAt

            });
            if (RequestList == null)
            {
                throw new Exception("Authentication Request not Exist");
            }
            return RequestList.ToList();
        }
    }
}