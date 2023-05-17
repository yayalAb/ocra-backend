using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventForDeathRequest : AddEventRequest
    {
        public virtual DeadPersonalInfoDTO EventOwener { get; set; }
        public RegistrarForDeathRequest EventRegistrar { get; set; }
    }
}