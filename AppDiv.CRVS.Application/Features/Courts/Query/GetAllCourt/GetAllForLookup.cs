using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Courts.Query.GetAllCourt

{
    // Customer query with List<Customer> response
    public record GetAllForLookup : IRequest<List<CourtListDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllForLookupHandler : IRequestHandler<GetAllForLookup, List<CourtListDTO>>
    {
        private readonly ICourtRepository _courtRepository;

        public GetAllForLookupHandler(ICourtRepository courtQueryRepository)
        {
            _courtRepository = courtQueryRepository;
        }
        public async Task<List<CourtListDTO>> Handle(GetAllForLookup request, CancellationToken cancellationToken)
        {
            var lookuplist = _courtRepository.GetAll()
            .Include(a=>a.Address)
                                .Select(co => new CourtListDTO
                                {
                                    id = co.Id,
                                    Name = co.NameLang+", " + co.Address.AddressNameLang,
                                    Description = co.DescriptionLang,
                                });

            return lookuplist.ToList(); // List<CourtListDTO>
            ;
        }
    }
}