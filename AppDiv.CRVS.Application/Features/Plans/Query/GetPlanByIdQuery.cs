using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Plans.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetPlanByIdQuery : IRequest<PlanDTO>
    {
       public Guid Id { get; set; }
    }

    public class GetPlanByIdHandler : IRequestHandler<GetPlanByIdQuery, PlanDTO>
    {
        private readonly IPlanRepository _planRepository;
        private readonly IDateAndAddressService _addressService;

        public GetPlanByIdHandler(IPlanRepository planRepository, IDateAndAddressService addressService)
        {
            this._addressService = addressService;
            _planRepository = planRepository;
        }
        public async Task<PlanDTO> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
        {
            
            var selectedPlan = _planRepository
                .GetAll()
                .Include(p => p.EventPlans)
                .Where(p => p.Id == request.Id)
                .Select(p => new PlanDTO
                {
                    Id = p.Id,
                    AddressId = p.AddressId,
                    ParentPlanId = p.ParentPlanId,
                    PlannedDateEt = p.PlannedDateEt,
                    BudgetYear = p.BudgetYear,
                    MalePopulationSize = p.MalePopulationSize,
                    FemalePopulationSize = p.FemalePopulationSize,
                    PopulationSize = p.PopulationSize,
                    EventPlans = CustomMapper.Mapper.Map<List<UpdateEventPlan>>(p.EventPlans)
                }).SingleOrDefault();
            selectedPlan!.AddressResponseDTO = await _addressService.FormatedAddress(selectedPlan.AddressId)!;

            return selectedPlan;
        }
    }
}