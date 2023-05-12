using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventForDivorceRequest : AddEventRequest
    {
        public Guid? EventOwenerId { get; set; } = null;
        public DivorcePartnersInfoDTO EventOwener { get; set; }
        public Guid? EventRegistrarId { get; set; } = null;
        public RegistrarForDivorceRequest EventRegistrar { get; set; }
    }
}