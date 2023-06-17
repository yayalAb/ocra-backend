using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
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
        public Guid Id { get; private set; }

        public GetPlanByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetPlanByIdHandler : IRequestHandler<GetPlanByIdQuery, PlanDTO>
    {
        private readonly IPlanRepository _planRepository;

        public GetPlanByIdHandler(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }
        public async Task<PlanDTO> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
        {
            
            var selectedPlan = await _planRepository.GetAsync(request.Id);
            return CustomMapper.Mapper.Map<PlanDTO>(selectedPlan);
            // return selectedCustomer;
        }
    }
}