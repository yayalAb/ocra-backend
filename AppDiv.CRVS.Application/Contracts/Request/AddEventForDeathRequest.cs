using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventForDeathRequest : AddEventRequest
    {
        public Guid? EventOwenerId { get; set; } = null;
        public virtual DeadPersonalInfoDTO EventOwener { get; set; }
        public Guid? EventRegistrarId { get; set; } = null;
        public RegistrarForDeathRequest EventRegistrar { get; set; }
    }
}