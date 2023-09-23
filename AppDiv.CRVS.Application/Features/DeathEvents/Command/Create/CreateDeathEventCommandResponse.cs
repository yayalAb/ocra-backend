using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Create
{
    // Create Death event command response
    public class CreateDeathEventCommandResponse : BaseResponse
    {
        public bool IsManualRegistration { get; set; } = false;
        public Guid EventId { get; set; }
        public CreateDeathEventCommandResponse() : base()
        {

        }
    }
}
