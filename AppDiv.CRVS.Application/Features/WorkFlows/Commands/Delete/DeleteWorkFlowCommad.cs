using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.WorkFlows.Commands.Delete
{
    // Customer create command with string response
    public class DeleteWorkFlowCommad : IRequest<String>
    {
        public Guid Id { get; set; }


    }

    // Customer delete command handler with string response as output
    public class DeleteWorkFlowCommadHandler : IRequestHandler<DeleteWorkFlowCommad, String>
    {
        private readonly IWorkflowRepository _workflowRepository;
        public DeleteWorkFlowCommadHandler(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }

        public async Task<string> Handle(DeleteWorkFlowCommad request, CancellationToken cancellationToken)
        {
            try
            {
                var workFlowEntity = await _workflowRepository.GetByIdAsync(request.Id);

                await _workflowRepository.DeleteAsync(workFlowEntity);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Lookup information has been deleted!";
        }
    }
}