using System.Collections.Generic;
using System.Linq;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.WorkFlows.Commands.Create
{

    public class CreateWorkFlowCommandHandler : IRequestHandler<CreateWorkFlowCommand, CreateWorkFlowCommandResponse>
    {
        private readonly IWorkflowRepository _workflowRepository;
        public CreateWorkFlowCommandHandler(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<CreateWorkFlowCommandResponse> Handle(CreateWorkFlowCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var CreateWorkFlowCommandResponse = new CreateWorkFlowCommandResponse();

            var validator = new CreateWorkFlowCommandValidetor(_workflowRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                CreateWorkFlowCommandResponse.Success = false;
                CreateWorkFlowCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateWorkFlowCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateWorkFlowCommandResponse.Message = CreateWorkFlowCommandResponse.ValidationErrors[0];
            }
            if (CreateWorkFlowCommandResponse.Success)
            {
                //can use this instead of automapper


                var workflow = new Workflow
                {
                    Id = Guid.NewGuid(),
                    workflowName = request.workflow.workflowName,
                    Descreption = request.workflow.Descreption,
                    Steps = CustomMapper.Mapper.Map<ICollection<Step>>(request.workflow.Steps)
                };
                //
                await _workflowRepository.InsertAsync(workflow, cancellationToken);
                var result = await _workflowRepository.SaveChangesAsync(cancellationToken);

                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // CreateWorkFlowCommandResponse.Customer = customerResponse;          
            }
            return CreateWorkFlowCommandResponse;
        }
    }
}
