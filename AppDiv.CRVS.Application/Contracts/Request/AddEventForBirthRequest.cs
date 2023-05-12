using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventForBirthRequest : AddEventRequest
    {
        public Guid? EventOwenerId { get; set; } = null;
        public ChildInfoDTO EventOwener { get; set; }
        public RegistrarForBirthRequest EventRegistrar { get; set; }
    }
}