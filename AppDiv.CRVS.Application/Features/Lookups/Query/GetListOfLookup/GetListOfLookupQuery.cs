using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Query.GetListOfLookup
{
    // Customer GetListOfLookupQuery with  response
    public class GetListOfLookupQuery : IRequest<List<ListOfLookupDTO>>
    {
        public String[] list { get; set; }

        // public GetListOfLookupQuery(String[] list)
        // {
        //     this.list = list;
        // }

    }

    public class GetListOfLookupQueryHandler : IRequestHandler<GetListOfLookupQuery, List<ListOfLookupDTO>>
    {
        private readonly ILookupRepository _lookupRepository;

        public GetListOfLookupQueryHandler(ILookupRepository lookupQueryRepository)
        {
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<List<ListOfLookupDTO>> Handle(GetListOfLookupQuery request, CancellationToken cancellationToken)
        {
            List<ListOfLookupDTO> LookupList = new List<ListOfLookupDTO>();
            var lookups = await _lookupRepository.GetAllAsync();
            // var lookups1= lookups. Contains(x=>x.key,request.list);
            foreach (var key in request.list)
            {
                // _logger.LogCritical(key);
                var selectedlookup = lookups.Where(x => x.Key == key);
                LookupList.Add(new ListOfLookupDTO
                {
                    Key = key,
                    Value = CustomMapper.Mapper.Map<List<LookupDTO>>(selectedlookup)
                });
            }

            return CustomMapper.Mapper.Map<List<ListOfLookupDTO>>(LookupList);

        }
    }
}