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
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Update;

namespace AppDiv.CRVS.Application.Features.Customers.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetDeathEventByIdQuery : IRequest<DeathEventDTO>
    {
        public Guid Id { get; private set; }

        public GetDeathEventByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetDeathEventByIdHandler : IRequestHandler<GetDeathEventByIdQuery, DeathEventDTO>
    {
        private readonly IDeathEventRepository _deathEventRepository;

        public GetDeathEventByIdHandler(IDeathEventRepository deathEventRepository)
        {
            _deathEventRepository = deathEventRepository;
        }
        public async Task<DeathEventDTO> Handle(GetDeathEventByIdQuery request, CancellationToken cancellationToken)
        {

            var selectedDeathEvent = await _deathEventRepository.GetIncludedAsync(request.Id);
            return CustomMapper.Mapper.Map<DeathEventDTO>(selectedDeathEvent);
            // return selectedCustomer;
        }
    }
}