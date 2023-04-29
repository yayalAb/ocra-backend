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
    public class DeleteStepCommand : IRequest<String>
    {
        public Guid Id { get; set; }
        // public DeleteStepCommand(Guid id)
        // {
        //     this.Id = id;
        // }


    }

    // Customer delete command handler with string response as output
    public class DeleteStepCommandHandler : IRequestHandler<DeleteStepCommand, String>
    {
        private readonly IStepRepository _stepRepository;
        public DeleteStepCommandHandler(IStepRepository stepRepository)
        {
            _stepRepository = stepRepository;
        }

        public async Task<string> Handle(DeleteStepCommand request, CancellationToken cancellationToken)
        {
            try
            {

                await _stepRepository.DeleteAsync(request.Id);
                await _stepRepository.SaveChangesAsync(cancellationToken);

            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Step information has been deleted!";
        }
    }
}