
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
    public record GetAllLookupQuery : IRequest<List<LookupForGridDTO>>
    {
    }

    public class GetAllLookupQueryHandler : IRequestHandler<GetAllLookupQuery, List<LookupForGridDTO>>
    {
        private readonly ILookupRepository _lookupRepository;

        public GetAllLookupQueryHandler(ILookupRepository lookupQueryRepository)
        {
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<List<LookupForGridDTO>> Handle(GetAllLookupQuery request, CancellationToken cancellationToken)
        {
            var LookupCouch= _lookupRepository.GetAll()
                                    .Select(lo => new LookupForGridDTO
                                    {
                                        id = lo.Id,
                                        Key = lo.Key,
                                        ValueAm = lo.Value.Value<string>("am"),
                                        ValueOr = lo.Value.Value<string>("or"),
                                        StatisticCode = lo.StatisticCode,
                                        Code = lo.Code
                                    }).ToList();


            return LookupCouch;
        }
    }
}