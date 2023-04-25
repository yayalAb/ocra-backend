using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
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
        // public DeleteWorkFlowCommad(Guid id)
        // {
        //     this.Id = id;
        // }


    }

    // Customer delete command handler with string response as output
    public class DeleteWorkFlowCommadHandler : IRequestHandler<DeleteWorkFlowCommad, String>
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly ILogger<DeleteWorkFlowCommadHandler> _Ilog;

        public DeleteWorkFlowCommadHandler(IWorkflowRepository workflowRepository, ILogger<DeleteWorkFlowCommadHandler> Ilog)
        {
            _workflowRepository = workflowRepository;
            _Ilog = Ilog;
        }

        public async Task<string> Handle(DeleteWorkFlowCommad request, CancellationToken cancellationToken)
        {
            try
            {
                _Ilog.LogCritical(request.Id.ToString());
                var workFlowEntity = await _workflowRepository.GetByIdAsync(request.Id);

                await _workflowRepository.DeleteAsync(request.Id);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Workflow information has been deleted!";
        }
    }
}