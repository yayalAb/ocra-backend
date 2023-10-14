using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Create
{
    // Create Death event command response
    public class CreateDeathEventCommandResponse : BaseResponse
    {
        public bool IsManualRegistration { get; set; } = false;
        public Guid EventId { get; set; }
        public IDeathEventRepository? deathEventRepository;
        public CreateDeathEventCommandResponse() : base()
        {

        }
    }
}
