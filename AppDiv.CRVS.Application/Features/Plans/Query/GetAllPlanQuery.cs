using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Plans.Query
{
    // Customer query with List<Customer> response
    public record GetAllPlanQuery : IRequest<PaginatedList<PlanGridDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
    }

    public class GetAllPlanHandler : IRequestHandler<GetAllPlanQuery, PaginatedList<PlanGridDTO>>
    {
        private readonly IPlanRepository _planRepository;

        public GetAllPlanHandler(IPlanRepository planQueryRepository)
        {
            _planRepository = planQueryRepository;
        }
        public async Task<PaginatedList<PlanGridDTO>> Handle(GetAllPlanQuery request, CancellationToken cancellationToken)
        {
            var plans = _planRepository.GetEventPlans();
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                plans = plans.Where(
                    u => EF.Functions.Like(u.EventType, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Plan.BudgetYear.ToString(), "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.TargetAmount.ToString(), "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Plan.PlannedDateEt, "%" + request.SearchString + "%"));
            }
            return await plans.OrderByDescending(p => p.CreatedAt)
                .Select(p => new PlanGridDTO
                {
                    Id = p.Plan.Id,
                    AddressId = p.Plan.AddressId,
                    EventType = p.EventType,
                    Address = $@"{p.Plan.Address.ParentAddress!.ParentAddress!.AddressNameLang}/{p.Plan.Address.ParentAddress!.AddressNameLang}/{p.Plan.Address.AddressNameLang}".Trim('/'),
                    BudgetYear = p.Plan.BudgetYear,
                    PlannedDateEt = p.Plan.PlannedDateEt,
                    PopulationSize = p.Plan.PopulationSize
                })
            .PaginateAsync<PlanGridDTO, PlanGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}