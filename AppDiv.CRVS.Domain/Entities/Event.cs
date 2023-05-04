using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Event : BaseAuditableEntity
    {
        public string EventType { get; set; }
        public Guid EventOwenerId { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventRegDate { get; set; }
        public Guid EventAddressId { get; set; }
        public Address EventAddress { get; set; }
        public Guid InformantTypeLookupId { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public bool IsExampted { get; set; } = false;
        public bool IsCertified { get; set; } = false;
        public virtual PersonalInfo EventOwener { get; set; }
        public virtual PersonalInfo CivilRegOfficer { get; set; }
        public virtual BirthEvent BirthEvent { get; set; }
        public virtual Lookup InformantTypeLookup { get; set; }
        public virtual Registrar EventRegistrar { get; set; }

        public virtual DeathEvent DeathEventNavigation { get; set; }
        public virtual AdoptionEvent AdoptionEvent { get; set; }
        public virtual DivorceEvent DivorceEvent { get; set; }
    }
}