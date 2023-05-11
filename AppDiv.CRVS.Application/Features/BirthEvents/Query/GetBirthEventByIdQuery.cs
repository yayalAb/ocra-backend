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
    public class GetBirthEventByIdQuery : IRequest<BirthEventDTO>
    {
        public Guid Id { get; private set; }

        public GetBirthEventByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetBirthEventByIdHandler : IRequestHandler<GetBirthEventByIdQuery, BirthEventDTO>
    {
        private readonly IBirthEventRepository _BirthEventRepository;

        public GetBirthEventByIdHandler(IBirthEventRepository BirthEventRepository)
        {
            _BirthEventRepository = BirthEventRepository;
        }
        public async Task<BirthEventDTO> Handle(GetBirthEventByIdQuery request, CancellationToken cancellationToken)
        {

            var selectedBirthEvent = await _BirthEventRepository.GetWithIncludedAsync(request.Id);
            return CustomMapper.Mapper.Map<BirthEventDTO>(selectedBirthEvent);
            // return selectedCustomer;
        }
    }
}