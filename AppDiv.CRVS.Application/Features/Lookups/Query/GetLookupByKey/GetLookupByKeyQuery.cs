using AppDiv.CRVS.Application.Common;
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

namespace AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupByKey
{

    public class GetLookupByKeyQuery : IRequest<PaginatedList<LookupByKeyDTO>>
    {
        public string Key { get; set; }
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetLookupByKeyQueryHandler : IRequestHandler<GetLookupByKeyQuery, PaginatedList<LookupByKeyDTO>>
    {
        private readonly ILookupRepository _lookupRepository;

        public GetLookupByKeyQueryHandler(ILookupRepository lookupQueryRepository)
        {
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<PaginatedList<LookupByKeyDTO>> Handle(GetLookupByKeyQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<LookupByKeyDTO>
                            .CreateAsync(
                                 _lookupRepository.GetAll()
                                .Select(lo => new LookupByKeyDTO
                                {
                                    id = lo.Id,
                                    Key = lo.Key,
                                    Value = lo.Value.Value<string>("en")
                                }).ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}

// var lookups = await _lookupRepository.GetAllWithAsync(x => x.Key == request.Key);
// // var lookups = AllLookups.Where(x => x.Key == request.Key);
// var formatedLookup = lookups.Select(lo => new LookupByKeyDTO
// {
//     id = lo.Id,
//     Key = lo.Key,
//     Value = lo.Value.Value<string>("en")
// });



