
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup

{
    // Customer query with List<Customer> response
    public record GetAllLookupQuery : IRequest<PaginatedList<LookupForGridDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllLookupQueryHandler : IRequestHandler<GetAllLookupQuery, PaginatedList<LookupForGridDTO>>
    {
        private readonly ILookupRepository _lookupRepository;

        public GetAllLookupQueryHandler(ILookupRepository lookupQueryRepository)
        {
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<PaginatedList<LookupForGridDTO>> Handle(GetAllLookupQuery request, CancellationToken cancellationToken)
        {
            

            return await PaginatedList<LookupForGridDTO>
                            .CreateAsync(
                                 _lookupRepository.GetAll()
                                .Select(lo => new LookupForGridDTO
                                {
                                    id = lo.Id,
                                    Key = lo.Key,
                                    Value = lo.ValueLang,
                                    StatisticCode = lo.StatisticCode,
                                    Code = lo.Code
                                }).ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}