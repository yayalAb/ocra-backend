
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

namespace AppDiv.CRVS.Application.Features.Courts.Query.GetAllCourt

{
    // Customer query with List<Customer> response
    public record GetAllCourtQuery : IRequest<PaginatedList<CourtListDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllCourtQueryHandler : IRequestHandler<GetAllCourtQuery, PaginatedList<CourtListDTO>>
    {
        private readonly ICourtRepository _courtRepository;

        public GetAllCourtQueryHandler(ICourtRepository courtQueryRepository)
        {
            _courtRepository = courtQueryRepository;
        }
        public async Task<PaginatedList<CourtListDTO>> Handle(GetAllCourtQuery request, CancellationToken cancellationToken)
        {

            return await PaginatedList<CourtListDTO>
                            .CreateAsync(
                                 _courtRepository.GetAll()
                                .Select(co => new CourtListDTO
                                {
                                    id = co.Id,
                                    Name = co.NameLang,
                                    Description = co.DescriptionLang,
                                })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}