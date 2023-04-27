
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
    public class GetLookupByIdQuery : IRequest<LookupForGridDTO>
    {
        public Guid Id { get; private set; }

        public GetLookupByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetLookupByIdQueryHandler : IRequestHandler<GetLookupByIdQuery, LookupForGridDTO>
    {

        private readonly ILookupRepository _lookupRepository;
        private readonly IMediator _mediator;

        public GetLookupByIdQueryHandler(IMediator mediator, ILookupRepository lookupQueryRepository)
        {
            _mediator = mediator;
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<LookupForGridDTO> Handle(GetLookupByIdQuery request, CancellationToken cancellationToken)
        {
            var lookups = await _mediator.Send(new GetAllLookupQuery());
            var selectedlookup = lookups.FirstOrDefault(x => x.id == request.Id);
            return selectedlookup;
            // return selectedCustomer;
        }
    }
}