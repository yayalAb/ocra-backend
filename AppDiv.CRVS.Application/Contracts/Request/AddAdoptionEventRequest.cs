using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddAdoptionEventRequest : AddEventRequestBase
    {
        public AdoptionEventPersonalInfoDTO EventOwener { get; set; }
    }
}