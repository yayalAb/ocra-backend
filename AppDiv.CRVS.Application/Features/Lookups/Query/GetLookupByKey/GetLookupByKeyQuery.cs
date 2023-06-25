using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        public string? SearchString { get; set; }
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
            IQueryable<LookupByKeyDTO> lookups;

            if (request.Key.ToLower() == "facility-type")
            {
                var result = from a in _lookupRepository.GetAll().Where(x => x.Key == "facility")
                             from b in _lookupRepository.GetAll().Where(x => x.Key != "facility")
                             where EF.Functions.Like(a.ValueStr, "%\"en\": \"" + b.Key + "\"%")
                             || EF.Functions.Like(a.ValueStr, "%\"am\": \"" + b.Key + "\"%")
                             || EF.Functions.Like(a.ValueStr, "%\"or\": \"" + b.Key + "\"%")
                             select new { A = a, B = b };

                if (!string.IsNullOrEmpty(request.SearchString))
                {
                    result = result.Where(l => EF.Functions.Like(l.B.ValueStr, "%" + request.SearchString + "%"));
                }
                lookups = result
                   .Select(lo => new LookupByKeyDTO
                   {
                       id = lo.B.Id,
                       Key = lo.A.ValueLang,
                       Value = lo.B.ValueLang,
                   });
            }
            else
            {
                var results = _lookupRepository.GetAll().Where(x => x.Key == request.Key);
                if (!string.IsNullOrEmpty(request.SearchString))
                {
                    results = results.Where(l => EF.Functions.Like(l.ValueStr, "%" + request.SearchString + "%"));
                }
                lookups = results.Select(l => new LookupByKeyDTO
                {
                    id = l.Id,
                    Key = l.Key,
                    Value = l.ValueLang
                });

            }

            return await PaginatedList<LookupByKeyDTO>
                            .CreateAsync(
                                 lookups
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}




