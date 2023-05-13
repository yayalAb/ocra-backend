using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Certificates.Command.Create;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Customers.Query
{
    // Customer GenerateCustomerQuery with Customer response
    public class GenerateCertificateQuery : IRequest<JObject>
    {
        public Guid Id { get; private set; }

        public GenerateCertificateQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GenerateCertificateHandler : IRequestHandler<GenerateCertificateQuery, JObject>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IMediator _mediator;

        public GenerateCertificateHandler(ICertificateRepository CertificateRepository, IMediator mediator)
        {
            _certificateRepository = CertificateRepository;
        }
        public async Task<JObject> Handle(GenerateCertificateQuery request, CancellationToken cancellationToken)
        {
            var content = await _certificateRepository.GetContent(request.Id);
            var certificate = new CertificateRequest
            {
                EventId = request.Id,
                Content = content,
                PrintCount = 1
            };
            var createCertificate = new CreateCertificateCommand(certificate);
            var response = await _mediator.Send(createCertificate);

            return response.Success ? content : new JObject();
        }
    }
}