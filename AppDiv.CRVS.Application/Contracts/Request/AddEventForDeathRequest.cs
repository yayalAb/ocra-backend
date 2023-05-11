using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventForDeathRequest : AddEventRequest
    {
        public PersonalInfoForDeathDTO EventOwener { get; set; }
    }
}