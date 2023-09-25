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
    public record GetParentPlanQuery : IRequest<List<ParentPlanDropdownDTO>>
    {
        public Guid AddressId { get; set; }
    }

    public class GetParentPlanHandler : IRequestHandler<GetParentPlanQuery, List<ParentPlanDropdownDTO>>
    {
        private readonly IPlanRepository _planRepository;
        private readonly IDateAndAddressService _addressService;

        public GetParentPlanHandler(IPlanRepository planQueryRepository, IDateAndAddressService addressService)
        {
            _planRepository = planQueryRepository;
            this._addressService = addressService;
        }
        public async Task<List<ParentPlanDropdownDTO>> Handle(GetParentPlanQuery request, CancellationToken cancellationToken)
        {
            var address = await _addressService.FormatedAddress(request.AddressId)!;
            var plans = _planRepository.GetAll()
                .Where(p => (p.AddressId.ToString() == address.Kebele 
                    || p.AddressId.ToString() == address.Woreda 
                    || p.AddressId.ToString() == address.Zone 
                    || p.AddressId.ToString() == address.Region 
                    || p.AddressId.ToString() == address.Country) 
                    && p.AddressId != request.AddressId);
            
            return plans.Select(p => new ParentPlanDropdownDTO
                {
                    Plan = $"{p.Address.ParentAddress!.ParentAddress!.AddressNameLang}/{p.Address.ParentAddress!.AddressNameLang}/{p.Address.AddressNameLang}".Trim('/')
                         + $", {p.EventType}, {p.BudgetYear}"
                }).ToList();
        }
    }
}