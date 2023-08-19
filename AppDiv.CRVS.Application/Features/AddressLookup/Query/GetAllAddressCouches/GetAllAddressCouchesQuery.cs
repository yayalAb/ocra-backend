
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces.Persistence.Couch;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress

{
    // Customer query with List<Customer> response
    public record GetAllAddressCouchesQuery : IRequest<object>
    {

    }

    public class GetAllAddressCouchesQueryHandler : IRequestHandler<GetAllAddressCouchesQuery, object>
    {
        private readonly IAddressLookupCouchRepository _addressLookupCouchRepo;
        private readonly IAddressLookupRepository addressRepo;

        public GetAllAddressCouchesQueryHandler(IAddressLookupCouchRepository addressLookupCouchRepo, IAddressLookupRepository addressRepo)
        {
            _addressLookupCouchRepo = addressLookupCouchRepo;
            this.addressRepo = addressRepo;
        }
        public async Task<object> Handle(GetAllAddressCouchesQuery request, CancellationToken cancellationToken)
        {  

            return  addressRepo.InitializeAddressLookupCouch().Result;

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }
    }
}