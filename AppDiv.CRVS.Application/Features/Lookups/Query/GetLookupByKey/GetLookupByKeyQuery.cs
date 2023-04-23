using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupByKey
{

    public class GetLookupByKeyQuery : IRequest<List<LookupDTO>>
    {
        public string Key { get; set; }


    }

    public class GetLookupByKeyQueryHandler : IRequestHandler<GetLookupByKeyQuery, List<LookupDTO>>
    {
        private readonly IMediator _mediator;


        public GetLookupByKeyQueryHandler(IMediator mediator)
        {
            _mediator = mediator;

        }
        public async Task<List<LookupDTO>> Handle(GetLookupByKeyQuery request, CancellationToken cancellationToken)
        {
            var AllLookups = await _mediator.Send(new GetAllLookupQuery());


            var lookups = CustomMapper.Mapper.Map<List<LookupDTO>>(AllLookups.Where(x => x.Key == request.Key));

            return lookups;
        }
    }
}




