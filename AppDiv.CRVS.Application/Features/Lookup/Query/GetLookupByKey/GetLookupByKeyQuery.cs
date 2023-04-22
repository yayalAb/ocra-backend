using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookup.Query.GetAllLookup;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookup.Query.GetLookupByKey
{

    public class GetLookupByKeyQuery : IRequest<LookupModel>
    {
        public string Key { get; private set; }

        public GetLookupByKeyQuery(string email)
        {
            this.Key = Key;
        }
    }

    public class GetLookupByKeyQueryHandler : IRequestHandler<GetLookupByKeyQuery, LookupModel>
    {
        private readonly IMediator _mediator;

        public GetLookupByKeyQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<LookupModel> Handle(GetLookupByKeyQuery request, CancellationToken cancellationToken)
        {
            var lookups = await _mediator.Send(new GetAllLookupQuery());
            var selectedCustomer = lookups.FirstOrDefault(x => x.Key.ToLower().Contains(request.Key.ToLower()));

            return CustomMapper.Mapper.Map<LookupModel>(selectedCustomer);
        }
    }
}