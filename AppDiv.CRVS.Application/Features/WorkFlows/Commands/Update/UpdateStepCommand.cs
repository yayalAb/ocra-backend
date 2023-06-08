using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.WorkFlows.Commands.Update
{
    // Customer create command with CustomerResponse
    public class UpdateStepCommand : IRequest<StepDTO>
    {

        public Guid Id { get; set; }
        public int step { get; set; }
        public bool Status { get; set; }
        public JObject Description { get; set; }

        public Guid workflowId { get; set; }
        public virtual Guid? UserGroupId { get; set; }

    }

    public class UpdateStepCommandHandler : IRequestHandler<UpdateStepCommand, StepDTO>
    {
        private readonly IStepRepository _workflowRepository;
        public UpdateStepCommandHandler(IStepRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<StepDTO> Handle(UpdateStepCommand request, CancellationToken cancellationToken)
        {
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request);
            Step StepEntity = new Step
            {
                Id = request.Id,
                step = request.step,
                Status = request.Status,
                Description = request.Description,
                workflowId = request.workflowId,
                UserGroupId = request.UserGroupId
            };

            try
            {
                await _workflowRepository.UpdateAsync(StepEntity, x => x.Id);
                await _workflowRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var StepRespons = CustomMapper.Mapper.Map<StepDTO>(StepEntity);
            return StepRespons;
        }
    }
}