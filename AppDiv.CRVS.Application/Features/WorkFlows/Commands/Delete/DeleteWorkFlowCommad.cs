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
        // public DeleteWorkFlowCommad(Guid id)
        // {
        //     this.Id = id;
        // }


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
            try
            {
                var workFlowEntity = await _workflowRepository.GetByIdAsync(request.Id);

                await _workflowRepository.DeleteAsync(request.Id);
                await _workflowRepository.SaveChangesAsync(cancellationToken);

            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
            var res = new BaseResponse
            {
                Success = true,
                Message = "Step information has been deleted!"
            };

            return res;
        }
    }
}