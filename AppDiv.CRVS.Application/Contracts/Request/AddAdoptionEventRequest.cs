using AppDiv.CRVS.Application.Contracts.DTOs;
using static AppDiv.CRVS.Application.Contracts.Request.AdoptionPersonalINformationRequest;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddAdoptionEventRequest : AddEventRequestBase
    {
        public AddAdoptionPersonalInfoRequest EventOwener { get; set; }
    }
}