
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class RegistrarForDivorceRequest : AddRegistrarRequest
    {
        public Guid? RegistrarInfoId { get; set; } = null;
        public virtual DivorcePartnersInfoDTO RegistrarInfo { get; set; }
    }
}