

using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class PersonDuplicate : BaseAuditableEntity
    {
        public Guid OldPersonId { get; set; }
        public Guid NewPersonId { get; set; }
        public string FoundWith { get; set; }
        public string Status {get;set;}
        public string? CorrectedBy {get; set;}
        public virtual PersonalInfo OldPerson {get; set;}
        public virtual PersonalInfo NewPerson {get; set;}
    


    }
}