using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Interfaces;
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
    public record GetParentPlanQuery : IRequest<List<PlanGridDTO>>
    {
        public Guid AddressId { get; set; }
        public uint BudgetYear { get; set; }
    }

    public class GetParentPlanHandler : IRequestHandler<GetParentPlanQuery, List<PlanGridDTO>>
    {
        private readonly IPlanRepository _planRepository;
        private readonly IDateAndAddressService _addressService;
        private readonly IAddressLookupRepository _addressRepository;

        public GetParentPlanHandler(IPlanRepository planQueryRepository, IDateAndAddressService addressService, IAddressLookupRepository addressRepository)
        {
            this._addressRepository = addressRepository;
            _planRepository = planQueryRepository;
            this._addressService = addressService;
        }
        public async Task<List<PlanGridDTO>> Handle(GetParentPlanQuery request, CancellationToken cancellationToken)
        {
            var address = _addressRepository.GetSingle(request.AddressId);
            var formatedAddress = await _addressService.FormatedAddress(request.AddressId)!;
            var plans = _planRepository.GetEventPlans()
                .Where(p => (p.Plan.AddressId.ToString() == formatedAddress.Kebele 
                    || p.Plan.AddressId.ToString() == formatedAddress.Woreda 
                    || p.Plan.AddressId.ToString() == formatedAddress.Zone 
                    || p.Plan.AddressId.ToString() == formatedAddress.Region 
                    || p.Plan.AddressId.ToString() == formatedAddress.Country) 
                    && p.Plan.AddressId != request.AddressId
                    && p.Plan.BudgetYear == request.BudgetYear
                    && address.AdminLevel - 1 == p.Plan.Address.AdminLevel
                    );
            return plans.Select(p => new PlanGridDTO
                {
                    Id = p.Plan.Id,
                    AddressId = p.Plan.AddressId,
                    EventType = p.EventType,
                    TargetAmount = p.TargetAmount,
                    Address = $@"{p.Plan.Address.ParentAddress!.ParentAddress!.AddressNameLang}/{p.Plan.Address.ParentAddress!.AddressNameLang}/{p.Plan.Address.AddressNameLang}".Trim('/'),
                    BudgetYear = p.Plan.BudgetYear,
                    PlannedDateEt = p.Plan.PlannedDateEt,
                    PopulationSize = p.Plan.PopulationSize
                }).ToList();
        }
    }
}