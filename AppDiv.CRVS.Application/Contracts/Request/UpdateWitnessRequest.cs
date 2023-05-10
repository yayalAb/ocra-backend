using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Domain.Entities
{
    public class UpdateWitnessRequest
    {
        public Guid Id { get; set ;}
        public string WitnessFor { get; set;}
        
        public virtual UpdatePersonalInfoRequest WitnessPersonalInfo { get;set;}
    
    }
}