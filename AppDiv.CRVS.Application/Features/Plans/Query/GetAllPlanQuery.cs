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

namespace AppDiv.CRVS.Application.Features.Plans.Query
{
    // Customer query with List<Customer> response
    public record GetAllPlanQuery : IRequest<PaginatedList<PlanDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllPlanHandler : IRequestHandler<GetAllPlanQuery, PaginatedList<PlanDTO>>
    {
        private readonly IPlanRepository _planRepository;

        public GetAllPlanHandler(IPlanRepository planQueryRepository)
        {
            _planRepository = planQueryRepository;
        }
        public async Task<PaginatedList<PlanDTO>> Handle(GetAllPlanQuery request, CancellationToken cancellationToken)
        {

            return await PaginatedList<PlanDTO>
                            .CreateAsync(
                                _planRepository.GetAll().Select(pr => new PlanDTO
                                {
                                    Id = pr.Id,

                                }).ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}