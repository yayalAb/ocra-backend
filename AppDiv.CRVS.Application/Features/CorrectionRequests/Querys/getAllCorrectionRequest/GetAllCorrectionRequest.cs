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

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Querys.getAllCorrectionRequest
{
    // Customer GetAllCorrectionRequest with  response
    public class GetAllCorrectionRequest : IRequest<List<AddCorrectionRequest>>
    {
        public Guid CivilRegOfficerId { get; set; }
    }

    public class GetAllCorrectionRequestHandler : IRequestHandler<GetAllCorrectionRequest, List<AddCorrectionRequest>>
    {

        private readonly ICorrectionRequestRepostory _correctionRequestRepository;

        public GetAllCorrectionRequestHandler(ICorrectionRequestRepostory correctionRequestRepository)
        {
            _correctionRequestRepository = correctionRequestRepository;
        }
        public async Task<List<AddCorrectionRequest>> Handle(GetAllCorrectionRequest request, CancellationToken cancellationToken)
        {
            var CorrectionRequest = _correctionRequestRepository.GetAll()
            .Include(x => x.Request)
            .Where(x => x.Request.CivilRegOfficerId == request.CivilRegOfficerId);
            return CustomMapper.Mapper.Map<List<AddCorrectionRequest>>(CorrectionRequest);
        }
    }
}