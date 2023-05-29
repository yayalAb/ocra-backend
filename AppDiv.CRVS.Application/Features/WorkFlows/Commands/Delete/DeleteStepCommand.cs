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
            var res = new BaseResponse();
            try
            {
                var workFlowEntity = await _stepRepository.GetAsync(request.Id);
                // if(workFlowEntity.workflow.Id)
                await _stepRepository.DeleteAsync(request.Id);
                await _stepRepository.SaveChangesAsync(cancellationToken);
                res.Deleted("Step");

            }
            catch (Exception exp)
            {
                res.BadRequest("Unble to delete the specified step");
                // throw (new ApplicationException(exp.Message));
            }
            return res;
        }
    }
}