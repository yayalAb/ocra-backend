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
    public class UpdateWorkFlowCommand : IRequest<WorkflowDTO>
    {

        public Guid id { get; set; }
        public string workflowName { get; set; }
        public JObject Description { get; set; }
        public ICollection<StepDTO> Steps { get; set; }

    }

    public class UpdateWorkFlowCommandHandler : IRequestHandler<UpdateWorkFlowCommand, WorkflowDTO>
    {
        private readonly IWorkflowRepository _workflowRepository;
        public UpdateWorkFlowCommandHandler(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<WorkflowDTO> Handle(UpdateWorkFlowCommand request, CancellationToken cancellationToken)
        {
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request);
            Workflow WorkflowEntity = new Workflow
            {
                Id = request.id,
                workflowName = request.workflowName,
                Description = request.Description,
                Steps = CustomMapper.Mapper.Map<ICollection<Step>>(request.Steps)
            };
            try
            {
                await _workflowRepository.UpdateAsync(WorkflowEntity, x => x.Id);
                await _workflowRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var modifiedworkflow = await _workflowRepository.GetByIdAsync(request.id);
            var workflowResponse = CustomMapper.Mapper.Map<WorkflowDTO>(modifiedworkflow);
            return workflowResponse;
        }
    }
}