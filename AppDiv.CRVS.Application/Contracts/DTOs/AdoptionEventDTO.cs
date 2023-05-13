using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AdoptionEventDTO : AddEventRequestBase
    {
        public AdoptionEventPersonalInfoDTO EventOwener { get; set; }
    }
}
