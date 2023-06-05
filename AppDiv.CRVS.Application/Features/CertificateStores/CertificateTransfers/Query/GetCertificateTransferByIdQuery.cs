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

namespace AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetCertificateTransferByIdQuery : IRequest<CertificateTransferDTO>
    {
        public Guid Id { get; private set; }

        public GetCertificateTransferByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetCertificateTransferByIdHandler : IRequestHandler<GetCertificateTransferByIdQuery, CertificateTransferDTO>
    {
        private readonly ICertificateTransferRepository _certificateStoreRepository;

        public GetCertificateTransferByIdHandler(ICertificateTransferRepository certificateStoreRepository)
        {
            _certificateStoreRepository = certificateStoreRepository;
        }
        public async Task<CertificateTransferDTO> Handle(GetCertificateTransferByIdQuery request, CancellationToken cancellationToken)
        {
            var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
                                                {
                                                    { "PaymentTypeLookup", NavigationPropertyType.REFERENCE },
                                                    { "EventLookup", NavigationPropertyType.REFERENCE }
                                                };
            var selectedCertificateTransfer = await _certificateStoreRepository.GetWithAsync(request.Id, explicitLoadedProperties);
            return CustomMapper.Mapper.Map<CertificateTransferDTO>(selectedCertificateTransfer);
            // return selectedCustomer;
        }
    }
}