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
    public record GetAllPlanQuery : IRequest<PaginatedList<PlanDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
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
            var plans = _planRepository.GetPlans();
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                plans = plans.Where(
                    u => 
                         EF.Functions.Like(u.BudgetYear.ToString(), "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.PlannedDateEt, "%" + request.SearchString + "%")).OrderByDescending(p => p.CreatedAt);
            }
            return await plans.Select(p => new PlanDTO
                {
                    Id = p.Id,
                    AddressId = p.AddressId,
                    Address = $@"{p.Address.ParentAddress!.ParentAddress!.AddressNameLang}/{p.Address.ParentAddress!.AddressNameLang}/{p.Address.AddressNameLang}".Trim('/'),
                    BudgetYear = p.BudgetYear,
                    PlannedDateEt = p.PlannedDateEt,
                    PopulationSize = p.PopulationSize
                })
            .PaginateAsync<PlanDTO, PlanDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}