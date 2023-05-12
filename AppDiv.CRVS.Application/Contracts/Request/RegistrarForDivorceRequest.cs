using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class RegistrarForDivorceRequest : AddRegistrarRequest
    {
        public Guid? RegistrarInfoId { get; set; } = null;
        public virtual DivorcePartnersInfoDTO RegistrarInfo { get; set; }
    }
}