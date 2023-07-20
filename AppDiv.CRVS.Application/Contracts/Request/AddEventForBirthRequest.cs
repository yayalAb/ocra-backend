
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventForBirthRequest : AddEventRequest
    {
        // public Guid? EventOwenerId { get; set; } = null;
        public ChildInfoDTO EventOwener { get; set; }
        public string? InformantType { get; set; }
        public RegistrarForBirthRequest? EventRegistrar { get; set; } = null;
    }
}