
using AppDiv.CRVS.Application.Features.Lookup.Query.GetAllLookup;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookup.Query.GetLookupById
{
    // Customer GetLookupByIdQuery with  response
    public class GetLookupByIdQuery : IRequest<LookupModel>
    {
        public String Id { get; private set; }

        public GetLookupByIdQuery(String Id)
        {
            this.Id = Id;
        }

    }

    public class GetLookupByIdQueryHandler : IRequestHandler<GetLookupByIdQuery, LookupModel>
    {
        private readonly IMediator _mediator;

        public GetLookupByIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<LookupModel> Handle(GetLookupByIdQuery request, CancellationToken cancellationToken)
        {
            var lookups = await _mediator.Send(new GetAllLookupQuery());
            var selectedlookup = lookups.FirstOrDefault(x => x.Key == request.Id);
            return CustomMapper.Mapper.Map<LookupModel>(selectedlookup);
            // return selectedCustomer;
        }
    }
}