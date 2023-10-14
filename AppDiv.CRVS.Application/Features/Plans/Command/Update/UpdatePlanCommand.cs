using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using AppDiv.CRVS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Features.Plans.Command.Update
{
    // Customer create command with CustomerResponse
    public class UpdatePlanCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        public string PlannedDateEt { get; set; }
        public uint BudgetYear { get; set; }
        public Guid? ParentPlanId { get; set; }
        public Guid AddressId { get; set; }
        public long MalePopulationSize { get; set; }
        public long FemalePopulationSize { get; set; }
        public ICollection<UpdateEventPlan> EventPlans { get; set; }

    }

    public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand, BaseResponse>
    {
        private readonly IPlanRepository _planRepository;
        public UpdatePlanCommandHandler(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }
        public async Task<BaseResponse> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                var plan = CustomMapper.Mapper.Map<Plan>(request);
                plan.PopulationSize = request.MalePopulationSize + request.FemalePopulationSize;
                _planRepository.Update(plan);
                var result = await _planRepository.SaveChangesAsync(cancellationToken);
                response.Status = 200;
                response.Message = "Plan Updated Succesfully";
            }
            catch (Exception exp)
            {
                response.BadRequest("Unable to update the plan");
                response.ValidationErrors = new List<string> { exp.Message };
                return response;
                // throw new ApplicationException(exp.Message);
            }

            return response;
        }
    }
}