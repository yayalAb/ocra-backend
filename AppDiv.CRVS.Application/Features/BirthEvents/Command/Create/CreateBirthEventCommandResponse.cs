using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    // Response for birth create.
    public class CreateBirthEventCommandResponse : BaseResponse
    {
        public bool IsManualRegistration { get; set; } = false;
        public Guid EventId { get; set; }
        public IBirthEventRepository? birthEventRepository;

        public CreateBirthEventCommandResponse() : base()
        {

        }
    }
}
