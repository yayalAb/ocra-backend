using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create
{
    public class CreateAdoptionCommandResponse : BaseResponse
    {
        public bool IsManualRegistration {get;set;}=false;
        public Guid EventId {get;set;}
        public CreateAdoptionCommandResponse() : base()
        {


        }
    }
}