using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Customers.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetCertificateByIdQuery : IRequest<CertificateDTO>
    {
        public Guid Id { get; private set; }

        public GetCertificateByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetCertificateByIdHandler : IRequestHandler<GetCertificateByIdQuery, CertificateDTO>
    {
        private readonly ICertificateRepository _CertificateRepository;

        public GetCertificateByIdHandler(ICertificateRepository CertificateRepository)
        {
            _CertificateRepository = CertificateRepository;
        }
        public async Task<CertificateDTO> Handle(GetCertificateByIdQuery request, CancellationToken cancellationToken)
        {
            // var customers = await _mediator.Send(new GetAllCustomerQuery());
            var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
                                                {
                                                    { "Event", NavigationPropertyType.REFERENCE }

                                                };
            var selectedCertificate = await _CertificateRepository.GetWithAsync(request.Id, explicitLoadedProperties);
            return CustomMapper.Mapper.Map<CertificateDTO>(selectedCertificate);
            // return selectedCustomer;
        }
    }
}