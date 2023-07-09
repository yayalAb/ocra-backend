using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Delete
{
    // Birth Delete command with string response
    public class DeleteBirthEventCommand : IRequest<String>
    {
        public Guid Id { get; private set; }

        public DeleteBirthEventCommand(Guid Id)
        {
            this.Id = Id;
        }
    }

    // Birth delete command handler with string response as output
    public class DeleteBirthEventCommmandHandler : IRequestHandler<DeleteBirthEventCommand, String>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        public DeleteBirthEventCommmandHandler(IBirthEventRepository birthEventRepository)
        {
            _birthEventRepository = birthEventRepository;
        }

        public async Task<string> Handle(DeleteBirthEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var paymentRateEntity = await _birthEventRepository.GetAsync(request.Id);
                if (paymentRateEntity != null)
                {
                    await _birthEventRepository.DeleteAsync(request.Id);
                    await _birthEventRepository.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    return "There is no birth event with the specified id";
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Birth Event information has been deleted!";
        }
    }
}