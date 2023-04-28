using System.Text.RegularExpressions;
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
    public class GetListOfLookupQuery : IRequest<object>
    {
        public String[] list { get; set; }
    }

    public class GetListOfLookupQueryHandler : IRequestHandler<GetListOfLookupQuery, object>
    {
        private readonly ILookupRepository _lookupRepository;

        public GetListOfLookupQueryHandler(ILookupRepository lookupQueryRepository)
        {
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<object> Handle(GetListOfLookupQuery request, CancellationToken cancellationToken)
        {
            var sss = _lookupRepository.GetAll().Where(x => request.list.Contains(x.Key)).AsEnumerable().GroupBy(x => x.Key).ToDictionary(group => group.Key, group => group.Select(li => new ListLookupDto
            {
                Id = li.Id,
                Value = li.ValueLang
            }));

            return sss;

        }
    }
}





// object LookupList = new object();
// List<Object> LookupList1 = new List<Object>();
// foreach (var key in request.list)
// {
//     var selectedlookup = _lookupRepository.GetAll().Where(x => x.Key == key).Select(lo => new ListLookupDto
//     {
//         Id = lo.Id,
//         Value = lo.Value.Value<string>("en")
//     });
//     LookupList.Add(new LookupDTO
//     {
//         Key = key,
//         Value = CustomMapper.Mapper.Map<List<ListLookupDto>>(selectedlookup)
//     });
// }

// CustomMapper.Mapper.Map<object>(sss);