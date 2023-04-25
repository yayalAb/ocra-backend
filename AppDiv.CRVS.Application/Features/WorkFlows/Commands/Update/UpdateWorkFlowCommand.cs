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
        public JObject Descreption { get; set; }
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
            Workflow groupEntity = new Workflow
            {
                Id = request.id,
                workflowName = request.workflowName,
                Descreption = request.Descreption,
                Steps = CustomMapper.Mapper.Map<List<Step>>(request.Steps),

            };
            try
            {
                await _workflowRepository.UpdateAsync(groupEntity, x => x.Id);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var modifiedLookup = await _workflowRepository.GetByIdAsync(request.id);
            var LookupResponse = CustomMapper.Mapper.Map<WorkflowDTO>(modifiedLookup);
            return LookupResponse;
        }
    }
}