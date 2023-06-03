using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Querys.GetForApproval
{
    // Customer GetCorrectionRequestForApproval with  response
    public class GetCorrectionRequestForApproval : IRequest<List<AddCorrectionRequest>>
    {

    }

    public class GetCorrectionRequestForApprovalHandler : IRequestHandler<GetCorrectionRequestForApproval, List<AddCorrectionRequest>>
    {

        private readonly ICorrectionRequestRepostory _correctionRequestRepository;

        public GetCorrectionRequestForApprovalHandler(ICorrectionRequestRepostory correctionRequestRepository)
        {
            _correctionRequestRepository = correctionRequestRepository;
        }
        public async Task<List<AddCorrectionRequest>> Handle(GetCorrectionRequestForApproval request, CancellationToken cancellationToken)
        {
            var CorrectionRequest = _correctionRequestRepository.GetAll()
            .Include(x => x.Request);
            return CustomMapper.Mapper.Map<List<AddCorrectionRequest>>(CorrectionRequest);
        }
    }
}