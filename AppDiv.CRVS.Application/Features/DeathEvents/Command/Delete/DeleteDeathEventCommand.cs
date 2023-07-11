using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Delete
{
    // Customer create command with string response
    public class DeleteDeathEventCommand : IRequest<String>
    {
        public Guid Id { get; private set; }

        public DeleteDeathEventCommand(Guid Id)
        {
            this.Id = Id;
        }
    }

    // Customer delete command handler with string response as output
    public class DeleteDeathEventCommmandHandler : IRequestHandler<DeleteDeathEventCommand, String>
    {
        private readonly IDeathEventRepository _deathEventRepository;
        public DeleteDeathEventCommmandHandler(IDeathEventRepository deathEventRepository)
        {
            _deathEventRepository = deathEventRepository;
        }

        public async Task<string> Handle(DeleteDeathEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var paymentRateEntity = await _deathEventRepository.GetAsync(request.Id);
                if (paymentRateEntity != null)
                {
                    await _deathEventRepository.DeleteAsync(request.Id);
                    await _deathEventRepository.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    return "There is no death with the specified id";
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Death Event information has been deleted!";
        }
    }
}