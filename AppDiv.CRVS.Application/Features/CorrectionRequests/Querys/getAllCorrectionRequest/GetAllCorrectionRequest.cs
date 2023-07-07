using AppDiv.CRVS.Application.Common;
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
    public class GetAllCorrectionRequest : IRequest<PaginatedList<CorrectionRequestListDTO>>
    {
        public Guid CivilRegOfficerId { get; set; }
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
    }

    public class GetAllCorrectionRequestHandler : IRequestHandler<GetAllCorrectionRequest, PaginatedList<CorrectionRequestListDTO>>
    {

        private readonly ICorrectionRequestRepostory _correctionRequestRepository;

        public GetAllCorrectionRequestHandler(ICorrectionRequestRepostory correctionRequestRepository)
        {
            _correctionRequestRepository = correctionRequestRepository;
        }
        public async Task<PaginatedList<CorrectionRequestListDTO>> Handle(GetAllCorrectionRequest request, CancellationToken cancellationToken)
        {
            var CorrectionRequest = _correctionRequestRepository.GetAll()
            .Include(x => x.Request)
            .Include(x => x.Event)
            .ThenInclude(x => x.CivilRegOfficer)
            .Where(x => x.Request.CivilRegOfficerId == request.CivilRegOfficerId);
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                CorrectionRequest = CorrectionRequest.Where(
                    u => EF.Functions.Like(u.Request.CivilRegOfficer.FirstNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Request.CivilRegOfficer.MiddleNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Request.CivilRegOfficer.LastNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Request.RequestType, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Event.EventType, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.CreatedAt.ToString(), "%" + request.SearchString + "%"));
            }
            var correctionRequestDto = CorrectionRequest.OrderByDescending(x => x.CreatedAt).Select(x => new CorrectionRequestListDTO
            {
                Id = x.Id,
                Requestedby = x.Request.CivilRegOfficer.FirstNameLang + " " + x.Request.CivilRegOfficer.MiddleNameLang + " " + x.Request.CivilRegOfficer.LastNameLang,
                RequestType = x.Request.RequestType,
                EventType = x.Event.EventType,
                RequestDate = x.CreatedAt,
                CurrentStatus = x.Request.currentStep,
                CanEdit = x.Request.currentStep == 0,
            });
            return await PaginatedList<CorrectionRequestListDTO>
                           .CreateAsync(
                                correctionRequestDto
                               , request.PageCount ?? 1, request.PageSize ?? 10);


            // CustomMapper.Mapper.Map<PaginatedList<CorrectionRequestListDTO>>(CorrectionRequest);
        }
    }
}

