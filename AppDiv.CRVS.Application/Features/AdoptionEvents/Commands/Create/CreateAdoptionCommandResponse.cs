using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create
{
    public class CreateAdoptionCommandResponse : BaseResponse
    {
        public bool IsManualRegistration {get;set;}=false;
        public Guid EventId {get;set;}
        public IAdoptionEventRepository? adoptionEventRepository;

        public CreateAdoptionCommandResponse() : base()
        {


        }
    }
}