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

namespace AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupByKeyForDropDown
{

    public class GetLookupByKeyForDropDownQuery : IRequest<List<LookupByKeyDTO>>
    {
        public string Key { get; set; }

    }

    public class GetLookupByKeyForDropDownQueryHandler : IRequestHandler<GetLookupByKeyForDropDownQuery, List<LookupByKeyDTO>>
    {
        private readonly ILookupRepository _lookupRepository;

        public GetLookupByKeyForDropDownQueryHandler(ILookupRepository lookupQueryRepository)
        {
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<List<LookupByKeyDTO>> Handle(GetLookupByKeyForDropDownQuery request, CancellationToken cancellationToken)
        {

            var LookupList = _lookupRepository.GetAll().Where(x => x.Key == request.Key)
                                .Select(lo => new LookupByKeyDTO
                                {
                                    id = lo.Id,
                                    Key = lo.Key,
                                    Value = lo.ValueLang
                                });
            return LookupList.ToList();


        }
    }
}




