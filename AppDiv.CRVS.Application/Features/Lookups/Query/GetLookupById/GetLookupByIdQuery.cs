
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupById
{
    // Customer GetLookupByIdQuery with  response
    public class GetLookupByIdQuery : IRequest<LookupDTO>
    {
        public Guid Id { get; private set; }

        public GetLookupByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetLookupByIdQueryHandler : IRequestHandler<GetLookupByIdQuery, LookupDTO>
    {

        private readonly ILookupRepository _lookupRepository;

        public GetLookupByIdQueryHandler(ILookupRepository lookupQueryRepository)
        {
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<LookupDTO> Handle(GetLookupByIdQuery request, CancellationToken cancellationToken)
        {
            // var lookups = await _mediator.Send(new GetAllLookupQuery());
            var selectedlookup = await _lookupRepository.GetAsync(request.Id);
            return CustomMapper.Mapper.Map<LookupDTO>(selectedlookup);
            // return selectedCustomer;
        }
    }
}