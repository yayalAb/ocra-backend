using AppDiv.CRVS.Application.Contracts.DTOs;
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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Certificates.Query
{
    public class CertificateResponseDTO
    {
        public JObject Content { get; set; }
        public Guid? TemplateId { get; set; }

    }

    // Customer GenerateCustomerQuery with Customer response
    public class GenerateCertificateQuery : IRequest<object>
    {
        public Guid Id { get; set; }
        // public bool Status { get; set; }
        // public bool AuthenticationStatus { get; set; }
        public string? CertificateSerialNumber { get; set; }
        public bool IsPrint { get; set; } = false;

        // public GenerateCertificateQuery(Guid Id, string SerialNumber)
        // {
        //     this.Id = Id;
        //     this.CertificateSerialNumber = SerialNumber;
        // }

    }

    public class GenerateCertificateHandler : IRequestHandler<GenerateCertificateQuery, object>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IBirthEventRepository _IBirthEventRepository;
        private readonly IMediator _mediator;
        private readonly ICertificateTemplateRepository _ICertificateTemplateRepository;
        private readonly ICertificateGenerator _CertificateGenerator;

        public GenerateCertificateHandler(ICertificateGenerator CertificateGenerator, IBirthEventRepository IBirthEventRepository, ICertificateTemplateRepository ICertificateTemplateRepository, ICertificateRepository CertificateRepository, IMediator mediato, IEventRepository eventRepository)
        {
            _certificateRepository = CertificateRepository;
            _eventRepository = eventRepository;
            _ICertificateTemplateRepository = ICertificateTemplateRepository;
            _IBirthEventRepository = IBirthEventRepository;
            _CertificateGenerator = CertificateGenerator;
        }
        public async Task<object> Handle(GenerateCertificateQuery request, CancellationToken cancellationToken)
        {
            var selectedEvent = await _eventRepository.GetByIdAsync(request.Id);
            var birthCertificateNo = _IBirthEventRepository.GetAll().Where(x => x.Event.EventOwenerId == selectedEvent.EventOwenerId).FirstOrDefault();
            // certificate.Content.
            var content = await _certificateRepository.GetContent(request.Id);
            var certificate = _CertificateGenerator.GetCertificate(request, content, birthCertificateNo?.Event?.CertificateId);

            var certificateTemplateId = _ICertificateTemplateRepository.GetAll().Where(c => c.CertificateType == selectedEvent.EventType).FirstOrDefault();
            if (request.IsPrint)
            {
                selectedEvent.IsCertified = true;
                _eventRepository.Update(CustomMapper.Mapper.Map<Event>(selectedEvent));
                await _certificateRepository.InsertAsync(certificate, cancellationToken);
                var result = await _certificateRepository.SaveChangesAsync(cancellationToken);
            }
            var Response = new CertificateResponseDTO()
            {
                Content = certificate.Content,
                TemplateId = certificateTemplateId?.Id
            };
            return Response;
        }
    }
}