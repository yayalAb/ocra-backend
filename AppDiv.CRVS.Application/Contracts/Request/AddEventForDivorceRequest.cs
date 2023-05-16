
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventForDivorceRequest : AddEventRequest
    {
        public DivorcePartnersInfoDTO EventOwener { get; set; }
        // public RegistrarForDivorceRequest? EventRegistrar { get; set; }
    }
}