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
    }
    public class DeleteStepCommandHandler : IRequestHandler<DeleteStepCommand, BaseResponse>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IWorkflowRepository _workflowRepository;

        public DeleteStepCommandHandler(IStepRepository stepRepository, IWorkflowRepository workflowRepository)
        {
            _stepRepository = stepRepository;
            _workflowRepository = workflowRepository;
        }

        public async Task<BaseResponse> Handle(DeleteStepCommand request, CancellationToken cancellationToken)
        {
            var res = new BaseResponse();
            try
            {

                await _stepRepository.DeleteAsync(request.Id);
                await _stepRepository.SaveChangesAsync(cancellationToken);
                res.Deleted("Step");
            }
            catch (Exception exp)
            {
                res.BadRequest("Unble to delete the specified step");
            }
            return res;
        }
    }
}

// var step = _stepRepository.GetAll().Where(x => x.Id == request.Id).FirstOrDefault();
//                 var steps = _stepRepository.GetAll().Where(x => x.workflowId == step.workflowId);
//                 if (steps.Count() == 1)
//                 {
//                     try
//                     {
//                         Console.WriteLine("It is last Step Work Flow Is Deleted34435345 {0} ", steps.Count());

//                         await _workflowRepository.DeleteAsync(step.workflowId);
//                     }
//                     catch (Exception ex)
//                     {
//                         Console.WriteLine("It is last Step Work Flow Is Deleted {0}43534534et ", steps.Count());

//                         throw new Exception("Unble to delete the specified step, B/c The Work Flow Is Used");
//                     }
//                 }