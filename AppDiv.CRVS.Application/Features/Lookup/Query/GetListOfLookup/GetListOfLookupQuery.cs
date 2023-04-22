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

namespace AppDiv.CRVS.Application.Features.Lookup.Query.GetListOfLookup
{
    // Customer GetListOfLookupQuery with  response
    public class GetListOfLookupQuery : IRequest<ListOfLookupDTO>
    {
        public String[] list { get; private set; }

        public GetListOfLookupQuery(String[] list)
        {
            this.list = list;
        }

    }

    public class GetListOfLookupQueryHandler : IRequestHandler<GetListOfLookupQuery, ListOfLookupDTO>
    {
        private readonly IMediator _mediator;

        public GetListOfLookupQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<ListOfLookupDTO> Handle(GetListOfLookupQuery request, CancellationToken cancellationToken)
        {
            var LookupList = new List<ListOfLookupDTO>();
            var lookup = new ListOfLookupDTO();
            var lookups = await _mediator.Send(new GetAllLookupQuery());
            for (int i = 0; i < request.list.Length; i++)
            {
                var selectedlookup = lookups.Where(x => x.Key == request.list[i]);
                lookup.Key = request.list[i];
                lookup.Value = selectedlookup;
                LookupList.Add(lookup);
            }

            return CustomMapper.Mapper.Map<ListOfLookupDTO>(LookupList);

        }
    }
}