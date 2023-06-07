using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Archives.Query;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Service.ArchiveService;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Querys.GetForApproval
{
    // Customer GetCorrectionRequestForApproval with  response
    public class GetCorrectionRequestForApproval : IRequest<CorrectionApprovalDTO>
    {
        public Guid CorrectionRequestId { get; set; }


    }

    public class GetCorrectionRequestForApprovalHandler : IRequestHandler<GetCorrectionRequestForApproval, CorrectionApprovalDTO>
    {

        private readonly ICorrectionRequestRepostory _correctionRequestRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IArchiveGenerator _archiveGenerator;
        private readonly IBirthEventRepository _IBirthEventRepository;



        public GetCorrectionRequestForApprovalHandler(IBirthEventRepository IBirthEventRepository, IArchiveGenerator archiveGenerator, ICorrectionRequestRepostory correctionRequestRepository, IEventRepository eventRepository)
        {
            _correctionRequestRepository = correctionRequestRepository;
            _eventRepository = eventRepository;
            _archiveGenerator = archiveGenerator;
            _IBirthEventRepository = IBirthEventRepository;
        }
        public async Task<CorrectionApprovalDTO> Handle(GetCorrectionRequestForApproval request, CancellationToken cancellationToken)
        {

            // var certificateTemplateId = _ICertificateTemplateRepository.GetAll().Where(c => c.CertificateType == selectedEvent.EventType + " " + "Archive").FirstOrDefault();

            // response.Content = certificate;
            // response.TemplateId = certificateTemplateId?.Id;
            // return response;


            GenerateArchiveQuery request1 = new GenerateArchiveQuery
            {
                Id = request.CorrectionRequestId,
                CertificateSerialNumber = "",
                IsPrint = false
            };

            var CorrectionRequest = _correctionRequestRepository.GetAll().Where(x => x.Id == request.CorrectionRequestId)
            .Include(x => x.Request).FirstOrDefault();
            var content = await _eventRepository.GetArchive(CorrectionRequest.EventId);
            var selectedEvent = await _eventRepository.GetByIdAsync(CorrectionRequest.EventId);
            var birthCertificateNo = _IBirthEventRepository.GetAll().Where(x => x.Event.EventOwenerId == selectedEvent.EventOwenerId).FirstOrDefault();
            var certificate = _archiveGenerator.GetArchive(request1, content, birthCertificateNo?.Event?.CertificateId);

            var response = new CorrectionApprovalDTO();
            response.OldData = certificate;

            response.CurrentStep = CorrectionRequest?.Request.currentStep;
            var eventContent = _correctionRequestRepository.GetAll().Where(cr => cr.EventId == CorrectionRequest.EventId)
                                                        .FirstOrDefault()?.Content;
            response.NewData = selectedEvent.EventType switch
            {
                "Birth" => _archiveGenerator.GetBirthArchivePreview(ReturnArchiveFromJObject.GetArchive<BirthEvent>(eventContent), birthCertificateNo?.Event?.CertificateId),
                "Death" => _archiveGenerator.GetDeathArchivePreview(ReturnArchiveFromJObject.GetArchive<DeathEvent>(eventContent), birthCertificateNo?.Event?.CertificateId),
                "Adoption" => _archiveGenerator.GetAdoptionArchivePreview(ReturnArchiveFromJObject.GetArchive<AdoptionEvent>(eventContent), birthCertificateNo?.Event?.CertificateId),
                "Divorce" => _archiveGenerator.GetDivorceArchivePreview(ReturnArchiveFromJObject.GetArchive<DivorceEvent>(eventContent), birthCertificateNo?.Event?.CertificateId),
                "Marriage" => _archiveGenerator.GetMarriageArchivePreview(ReturnArchiveFromJObject.GetArchive<MarriageEvent>(eventContent), birthCertificateNo?.Event?.CertificateId),
            };

            return response;
        }
    }
}