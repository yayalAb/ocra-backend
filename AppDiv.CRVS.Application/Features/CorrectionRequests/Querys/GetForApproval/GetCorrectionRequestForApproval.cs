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
using Microsoft.AspNetCore.Components.Server;
using AppDiv.CRVS.Utility.Services;

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
            GenerateArchiveQuery request1 = new GenerateArchiveQuery
            {
                Id = request.CorrectionRequestId,
                CertificateSerialNumber = "",
                IsPrint = false
            };

            var CorrectionRequest = _correctionRequestRepository.GetAll().Include(cr => cr.Request.Notification)
            .ThenInclude(n => n.Sender).ThenInclude(s => s.PersonalInfo)
            .Where(x => x.Id == request.CorrectionRequestId)
            .Include(x => x.Request).FirstOrDefault();
            var content = await _eventRepository.GetArchive(CorrectionRequest.EventId);
            var selectedEvent = await _eventRepository.GetByIdAsync(CorrectionRequest.EventId);
            var birthCertificateNo = _IBirthEventRepository.GetAll().Where(x => x.Event.EventOwenerId == selectedEvent.EventOwenerId).FirstOrDefault();
            var certificate = _archiveGenerator.GetArchive(request1, content, birthCertificateNo?.Event?.CertificateId);
            var response = new CorrectionApprovalDTO();
            response.OldData = certificate;

            response.CurrentStep = CorrectionRequest?.Request.currentStep;
            var eventContent = _correctionRequestRepository.GetAll().Where(cr => cr.EventId == CorrectionRequest.EventId).FirstOrDefault()?.Content;
            response.NewData = selectedEvent.EventType switch
            {
                "Birth" => _archiveGenerator.GetBirthArchivePreview(ReturnArchiveFromJObject.GetArchive<BirthEvent>(CorrectionRequest.Content), birthCertificateNo?.Event?.CertificateId, true),
                "Death" => _archiveGenerator.GetDeathArchivePreview(ReturnArchiveFromJObject.GetArchive<DeathEvent>(CorrectionRequest.Content), birthCertificateNo?.Event?.CertificateId, true),
                "Adoption" => _archiveGenerator.GetAdoptionArchivePreview(ReturnArchiveFromJObject.GetArchive<AdoptionEvent>(CorrectionRequest.Content), birthCertificateNo?.Event?.CertificateId, true),
                "Divorce" => _archiveGenerator.GetDivorceArchivePreview(ReturnArchiveFromJObject.GetArchive<DivorceEvent>(CorrectionRequest.Content), birthCertificateNo?.Event?.CertificateId, true),
                "Marriage" => _archiveGenerator.GetMarriageArchivePreview(ReturnArchiveFromJObject.GetArchive<MarriageEvent>(CorrectionRequest.Content), birthCertificateNo?.Event?.CertificateId, true),
            };
            if (CorrectionRequest.Request?.Notification != null)
            {
                response.NotificationData = new NotificationData
                {

                    Message = CorrectionRequest.Request.Notification.MessageStr,
                    ApprovalType = CorrectionRequest.Request.Notification.ApprovalType,
                    SenderId = CorrectionRequest.Request.Notification.SenderId,
                    SenderUserName = CorrectionRequest.Request.Notification.Sender.UserName,
                    SenderFullName = CorrectionRequest.Request.Notification.Sender.PersonalInfo.FirstNameLang + " " +
                                             CorrectionRequest.Request.Notification.Sender.PersonalInfo.MiddleNameLang + " " +
                                             CorrectionRequest.Request.Notification.Sender.PersonalInfo.LastNameLang,
                    Date = (new CustomDateConverter(CorrectionRequest.Request.Notification.CreatedAt)).ethiopianDate,
                };
        }


            return response;
        }
    }
}