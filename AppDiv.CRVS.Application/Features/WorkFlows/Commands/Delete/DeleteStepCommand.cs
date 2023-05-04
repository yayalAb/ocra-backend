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
    public class DeleteStepCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        // public DeleteStepCommand(Guid id)
        // {
        //     this.Id = id;
        // }


    }

    // Customer delete command handler with BaseResponse response as output
    public class DeleteStepCommandHandler : IRequestHandler<DeleteStepCommand, BaseResponse>
    {
        private readonly IStepRepository _stepRepository;
        public DeleteStepCommandHandler(IStepRepository stepRepository)
        {
            _stepRepository = stepRepository;
        }

        public async Task<BaseResponse> Handle(DeleteStepCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var workFlowEntity = await _stepRepository.GetAsync(request.Id);
                // if(workFlowEntity.workflow.Id)
                await _stepRepository.DeleteAsync(request.Id);
                await _stepRepository.SaveChangesAsync(cancellationToken);

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