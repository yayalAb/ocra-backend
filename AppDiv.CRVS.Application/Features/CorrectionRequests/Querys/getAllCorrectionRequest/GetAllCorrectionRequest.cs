using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
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
        private readonly IUserRepository _UserRepo;
        private readonly IUserResolverService _ResolverService;


        public GetAllCorrectionRequestHandler(IUserResolverService ResolverService,IUserRepository UserRepo,ICorrectionRequestRepostory correctionRequestRepository)
        {
            _correctionRequestRepository = correctionRequestRepository;
            _UserRepo = UserRepo;
            _ResolverService = ResolverService;
        }
        public async Task<PaginatedList<CorrectionRequestListDTO>> Handle(GetAllCorrectionRequest request, CancellationToken cancellationToken)
        {
           var userGroup = _UserRepo.GetAll()
            .Include(g => g.UserGroups)
            .Include(x=>x.Address)
            .Where(x => x.Id == _ResolverService.GetUserId()).FirstOrDefault();

            var CorrectionRequest = _correctionRequestRepository.GetAll()
            .Include(x => x.Request)
            .Include(x => x.Event.EventOwener)
            .Include(x=>x.Request)
            .ThenInclude(x=>x.Workflow)
            .ThenInclude(x=>x.Steps)
            .ThenInclude(x=>x.UserGroup)
            .Include(x => x.Event.CivilRegOfficer)
            .Where(x => (x.Request.CivilRegOfficerId == request.CivilRegOfficerId)
            &&(x.Request.currentStep!=x.Request.NextStep));
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
                Requestedby = x.Request.CivilRegOfficer.FullNameLang,
                OwnerFullName = x.Event.EventOwener.FullNameLang!,
                CertificateId = x.Event.CertificateId!,
                RequestType = x.Request.RequestType,
                EventType = x.Event.EventType,
                RegiteredDate=new CustomDateConverter(x.Event.EventRegDate).ethiopianDate,//x.Event._EventRegDateEt,
                ResponsbleGroup=x.Request.Workflow.Steps.Where(s=>s.step==x.Request.NextStep).Select(n=>n.UserGroup.GroupName).FirstOrDefault(),
                RequestDate = new CustomDateConverter(x.CreatedAt).ethiopianDate,
                CurrentStatus = x.Request.currentStep+1,
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

