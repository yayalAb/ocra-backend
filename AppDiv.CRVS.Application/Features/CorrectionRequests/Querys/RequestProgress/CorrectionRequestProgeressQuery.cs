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

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Querys.RequestProgress
{
    // Customer CorrectionRequestProgeressQuery with  response
    public class CorrectionRequestProgeressQuery : IRequest<AddCorrectionRequest>
    {
        public Guid OfficerId { get; set; }


    }

    public class CorrectionRequestProgeressQueryHandler : IRequestHandler<CorrectionRequestProgeressQuery, AddCorrectionRequest>
    {

        private readonly ICorrectionRequestRepostory _correctionRequestRepository;

        public CorrectionRequestProgeressQueryHandler(ICorrectionRequestRepostory correctionRequestRepository)
        {
            _correctionRequestRepository = correctionRequestRepository;
        }
        public async Task<AddCorrectionRequest> Handle(CorrectionRequestProgeressQuery request, CancellationToken cancellationToken)
        {
            var CorrectionRequest = _correctionRequestRepository.GetAll()
            .Include(x => x.Request).Where(x => x.Request.CivilRegOfficerId == request.OfficerId);

            return CustomMapper.Mapper.Map<AddCorrectionRequest>(CorrectionRequest);
        }
    }
}