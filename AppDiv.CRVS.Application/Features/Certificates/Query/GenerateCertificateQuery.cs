using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Certificates.Command.Create;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Domain.Entities;
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
    // Customer GenerateCustomerQuery with Customer response
    public class GenerateCertificateQuery : IRequest<JObject>
    {
        public Guid Id { get; set; }
        // public bool Status { get; set; }
        // public bool AuthenticationStatus { get; set; }
        public string CertificateSerialNumber { get; set; }

        // public GenerateCertificateQuery(Guid Id, string SerialNumber)
        // {
        //     this.Id = Id;
        //     this.CertificateSerialNumber = SerialNumber;
        // }

    }

    public class GenerateCertificateHandler : IRequestHandler<GenerateCertificateQuery, JObject>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMediator _mediator;

        public GenerateCertificateHandler(ICertificateRepository CertificateRepository, IMediator mediato)
        {
            _certificateRepository = CertificateRepository;
        }
        public async Task<JObject> Handle(GenerateCertificateQuery request, CancellationToken cancellationToken)
        {
            var content = await _certificateRepository.GetContent(request.Id);
            var certificate = CertificateGenerator.GetCertificate(request, content);

            // var createCertificate = new CreateCertificateCommand(certificate);
            await _certificateRepository.InsertAsync(certificate, cancellationToken);
            var result = await _certificateRepository.SaveChangesAsync(cancellationToken);
            // var response = Ok(await _mediator.Send(createCertificate));

            return result ? certificate.Content : new JObject();
        }
    }
}