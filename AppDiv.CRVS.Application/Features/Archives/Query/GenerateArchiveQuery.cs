using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Certificates.Command.Create;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;

namespace AppDiv.CRVS.Application.Features.Archives.Query
{
    public class ArchiveResponseDTO
    {
        public JObject Content { get; set; }
        public Guid? TemplateId { get; set; }

    }

    // Customer GenerateCustomerQuery with Customer response
    public class GenerateArchiveQuery : IRequest<object>
    {
        public Guid Id { get; set; }
        public string? CertificateSerialNumber { get; set; }
        public bool IsPrint { get; set; } = false;

    }

    public class GenerateArchiveHandler : IRequestHandler<GenerateArchiveQuery, object>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IBirthEventRepository _IBirthEventRepository;
        private readonly IMediator _mediator;
        private readonly ICertificateTemplateRepository _ICertificateTemplateRepository;
        private readonly IArchiveGenerator _archiveGenerator;
        private readonly ILogger<GenerateArchiveHandler> _ILogger;
        private readonly IFileService _fileService;
        private readonly ISupportingDocumentRepository _supportingDocumentRepository;


        public GenerateArchiveHandler(ILogger<GenerateArchiveHandler> ILogger,
                                        IArchiveGenerator archiveGenerator,
                                        IBirthEventRepository IBirthEventRepository,
                                        ICertificateTemplateRepository ICertificateTemplateRepository,
                                        ICertificateRepository CertificateRepository,
                                        IMediator mediato,
                                        IEventRepository eventRepository,
                                        IFileService fileService,
                                        ISupportingDocumentRepository supportingDocumentRepository)
        {
            _certificateRepository = CertificateRepository;
            _eventRepository = eventRepository;
            _ICertificateTemplateRepository = ICertificateTemplateRepository;
            _IBirthEventRepository = IBirthEventRepository;
            _archiveGenerator = archiveGenerator;
            _ILogger = ILogger;
            _fileService = fileService;
            _supportingDocumentRepository = supportingDocumentRepository;
        }
        public async Task<object> Handle(GenerateArchiveQuery request, CancellationToken cancellationToken)
        {
            var selectedEvent = await _eventRepository.GetByIdAsync(request.Id);
            var birthCertificateNo = _IBirthEventRepository.GetAll().Where(x => x.Event.EventOwenerId == selectedEvent.EventOwenerId).FirstOrDefault();
            var content = await _eventRepository.GetArchive(request.Id);
            _ILogger.LogCritical(content?.EventOwener?.Id.ToString());
            _ILogger.LogCritical(content?.DivorceEvent?.DivorcedWife?.Id.ToString());
            _ILogger.LogCritical(content?.MarriageEvent?.BrideInfo?.Id.ToString());
            var certificate = _archiveGenerator.GetArchive(request, content, birthCertificateNo?.Event?.CertificateId);
            var certificateTemplateId = _ICertificateTemplateRepository.GetAll().Where(c => c.CertificateType == selectedEvent.EventType).FirstOrDefault();
            // if (request.IsPrint && !string.IsNullOrEmpty(request.CertificateSerialNumber))
            // {
            //     selectedEvent.IsCertified = true;
            //     _eventRepository.Update(CustomMapper.Mapper.Map<Event>(selectedEvent));
            //     await _certificateRepository.InsertAsync(certificate, cancellationToken);
            //     var result = await _certificateRepository.SaveChangesAsync(cancellationToken);
            // }
            var response = new ArchiveResponseDTO();

            // if (selectedEvent.EventType == "Marriage" || content.marriage != null)
            // {
            //     (string Bride, string Groom) image = _supportingDocumentRepository.MarriageImage();
            // }
            response.Content = certificate;
            response.TemplateId = certificateTemplateId?.Id;
            return response;
        }

    }
}