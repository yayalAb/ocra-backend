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
    public record GetParentPlanQuery : IRequest<PlanDTO>
    {
        public Guid AddressId { get; set; }
        public string? EventType { get; set; }
        public uint BudgetYear { get; set; }
    }

    public class GetParentPlanHandler : IRequestHandler<GetParentPlanQuery, PlanDTO>
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
        public async Task<PlanDTO> Handle(GetParentPlanQuery request, CancellationToken cancellationToken)
        {
            var address = _addressRepository.GetSingle(request.AddressId);
            var formatedAddress = await _addressService.FormatedAddress(request.AddressId)!;
            var plans = _planRepository.GetAll()
                .Where(p => (p.AddressId.ToString() == formatedAddress.Kebele 
                    || p.AddressId.ToString() == formatedAddress.Woreda 
                    || p.AddressId.ToString() == formatedAddress.Zone 
                    || p.AddressId.ToString() == formatedAddress.Region 
                    || p.AddressId.ToString() == formatedAddress.Country) 
                    && p.AddressId != request.AddressId
                    && p.EventType == request.EventType
                    && p.BudgetYear == request.BudgetYear
                    // && address.AdminLevel + 1 == p.Address.AdminLevel
                    );
            return plans.Select(p => new PlanDTO
                {
                    Id = p.Id,
                    ActualOccurance = p.ActualOccurance,
                    AddressId = p.AddressId,
                    Address = $@"{p.Address.ParentAddress!.ParentAddress!.AddressNameLang}/{p.Address.ParentAddress!.AddressNameLang}/{p.Address.AddressNameLang}".Trim('/'),
                    TargetAmount = p.TargetAmount,
                    BudgetYear = p.BudgetYear,
                    PlannedDateEt = p.PlannedDateEt,
                    EventType = p.EventType,
                    PopulationSize = p.PopulationSize
                }).SingleOrDefault()!;
            
            // return plans.Select(p => new ParentPlanDropdownDTO
            //     {
            //         Id = p.Id,
            //         Plan = ($"{p.Address.ParentAddress!.ParentAddress!.AddressNameLang}/" +
            //                 $"{p.Address.ParentAddress!.AddressNameLang}/" +
            //                 $"{p.Address.AddressNameLang}").Trim('/')
            //               + $", {p.EventType}, {p.BudgetYear}",
            //         TargetAmount = p.TargetAmount
            //     }).ToList();
        }
    }
}