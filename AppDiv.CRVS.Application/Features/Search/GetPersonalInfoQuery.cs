using System;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.Search
{
    // Customer GetPersonalInfoQuery with  response
    public class GetPersonalInfoQuery : IRequest<object>
    {
        public string SearchString { get; set; }
        public string? gender { get; set; }
        public int age { get; set; }


    }

    public class GetPersonalInfoQueryHandler : IRequestHandler<GetPersonalInfoQuery, object>
    {
        private readonly IPersonalInfoRepository _PersonaInfoRepository;

        public GetPersonalInfoQueryHandler(IPersonalInfoRepository PersonaInfoRepository)
        {
            _PersonaInfoRepository = PersonaInfoRepository;
        }
        public async Task<object> Handle(GetPersonalInfoQuery request, CancellationToken cancellationToken)
        {
           return await _PersonaInfoRepository.SearchPersonalInfo(request);
 

        }
    }
}