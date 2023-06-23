using AppDiv.CRVS.Application.Common;
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
    // Customer create command with BaseResponse response
    public class DeleteWorkFlowCommad : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
    }

    // Customer delete command handler with BaseResponse response as output
    public class DeleteWorkFlowCommadHandler : IRequestHandler<DeleteWorkFlowCommad, BaseResponse>
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly ILogger<DeleteWorkFlowCommadHandler> _Ilog;

        public DeleteWorkFlowCommadHandler(IWorkflowRepository workflowRepository, ILogger<DeleteWorkFlowCommadHandler> Ilog)
        {
            _workflowRepository = workflowRepository;
            _Ilog = Ilog;
        }

        public async Task<BaseResponse> Handle(DeleteWorkFlowCommad request, CancellationToken cancellationToken)
        {
            var res = new BaseResponse();
            try
            {
                var workFlowEntity = await _workflowRepository.GetByIdAsync(request.Id);

                await _workflowRepository.DeleteAsync(request.Id);
                await _workflowRepository.SaveChangesAsync(cancellationToken);
                res.Deleted("Workflow");

            }
            catch (Exception exp)
            {
                res.BadRequest("Unable to delete the specified workflow");
                // throw (new ApplicationException(exp.Message));
            }

            return res;
        }
    }
}