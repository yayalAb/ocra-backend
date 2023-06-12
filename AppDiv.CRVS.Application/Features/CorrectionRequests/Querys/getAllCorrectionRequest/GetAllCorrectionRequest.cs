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
    public class GetAllCorrectionRequest : IRequest<List<CorrectionRequestListDTO>>
    {
        public Guid CivilRegOfficerId { get; set; }
    }

    public class GetAllCorrectionRequestHandler : IRequestHandler<GetAllCorrectionRequest, List<CorrectionRequestListDTO>>
    {

        private readonly ICorrectionRequestRepostory _correctionRequestRepository;

        public GetAllCorrectionRequestHandler(ICorrectionRequestRepostory correctionRequestRepository)
        {
            _correctionRequestRepository = correctionRequestRepository;
        }
        public async Task<List<CorrectionRequestListDTO>> Handle(GetAllCorrectionRequest request, CancellationToken cancellationToken)
        {
            var CorrectionRequest = _correctionRequestRepository.GetAll()
            .Include(x => x.Request)
            .ThenInclude(x => x.CivilRegOfficer)
            .Where(x => x.Request.CivilRegOfficerId == request.CivilRegOfficerId).Select(x => new CorrectionRequestListDTO
            {
                Id = x.Id,
                Requestedby = x.Request.CivilRegOfficer.FirstNameLang + " " + x.Request.CivilRegOfficer.MiddleNameLang + " " + x.Request.CivilRegOfficer.LastNameLang,
                RequestType = x.Request.RequestType,
                RequestDate = x.CreatedAt,
                CurrentStatus = x.Request.currentStep,
                CanEdit = x.Request.currentStep == 0,
            });
            return CustomMapper.Mapper.Map<List<CorrectionRequestListDTO>>(CorrectionRequest);
        }
    }
}

