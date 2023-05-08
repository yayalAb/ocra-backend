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
    public class GetCertificateByEventIdQuery : IRequest<IEnumerable<CertificateDTO>>
    {
        public Guid Id { get; private set; }

        public GetCertificateByEventIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetCertificateByEventIdHandler : IRequestHandler<GetCertificateByEventIdQuery, IEnumerable<CertificateDTO>>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetCertificateByEventIdHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }
        public async Task<IEnumerable<CertificateDTO>> Handle(GetCertificateByEventIdQuery request, CancellationToken cancellationToken)
        {
            // var customers = await _mediator.Send(new GetAllCustomerQuery());
            // var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
            //                                     {
            //                                         { "Event", NavigationPropertyType.REFERENCE }

            //                                     };
            var selectedCertificate = await _certificateRepository.GetByEventAsync(request.Id);
            return CustomMapper.Mapper.Map<IEnumerable<CertificateDTO>>(selectedCertificate);
            // return selectedCustomer;
        }
    }
}