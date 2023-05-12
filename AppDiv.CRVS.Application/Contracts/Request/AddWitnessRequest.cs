using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Domain.Entities
{
    public class AddWitnessRequest
    {
        public string WitnessFor { get; set; }

        public virtual WitnessInfoDTO WitnessPersonalInfo { get; set; }
    
    }
}