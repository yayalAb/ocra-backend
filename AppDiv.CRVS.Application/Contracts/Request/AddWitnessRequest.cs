using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Domain.Entities
{
    public class AddWitnessRequest
    {
        public Guid? Id {get; set; }= null;
        public Guid WitnessForLookupId { get; set; }
        public virtual WitnessInfoDTO WitnessPersonalInfo { get; set; }

        public DateTime? CreatedAt {get; set; }
        public Guid? CreatedBy {get; set; }
    
    }
}