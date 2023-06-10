
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Lookup : BaseAuditableEntity
    {
        public string Key { get; set; }
        public string ValueStr { get; set; }
        public string? DescriptionStr { get; set; }
        public string? StatisticCode { get; set; }
        public string? Code { get; set; }
        public bool IsSystemLookup { get; set; }= false;
        [NotMapped]
        public JObject? Description
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(DescriptionStr) ? "{}" : DescriptionStr);
            }
            set
            {
                DescriptionStr = value.ToString();
            }
        }
        [NotMapped]
        public JObject Value
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ValueStr) ? "{}" : ValueStr);
            }
            set
            {
                ValueStr = value.ToString();
            }
        }
        // public virtual ICollection<Address> AddressAdminLevelNavigation { get; set; }
        public virtual ICollection<Address> AddressAreaTypeNavigation { get; set; }
        public virtual ICollection<Address> AdminTypeNavigation { get; set; }
        public virtual ICollection<PersonalInfo> PersonSexNavigation { get; set; }
        public virtual ICollection<PersonalInfo> PersonPlaceOfBirthNavigation { get; set; }
        public virtual ICollection<PersonalInfo> PersonNationalityNavigation { get; set; }
        public virtual ICollection<PersonalInfo> PersonTitleNavigation { get; set; }
        public virtual ICollection<PersonalInfo> PersonReligionNavigation { get; set; }
        public virtual ICollection<PersonalInfo> PersonEducationalStatusNavigation { get; set; }
        public virtual ICollection<PersonalInfo> PersonTypeOfWorkNavigation { get; set; }
        public virtual ICollection<PersonalInfo> PersonMarriageStatusNavigation { get; set; }
        public virtual ICollection<PersonalInfo> PersonNationNavigation { get; set; }
        public virtual ICollection<BirthEvent> BirthFacilityTypeNavigation { get; set; }
        public virtual ICollection<BirthEvent> BirthFacilityNavigation { get; set; }
        public virtual ICollection<BirthEvent> BirthTypeOfBirthNavigation { get; set; }
        public virtual ICollection<BirthEvent> BirthPlaceOfBirthNavigation { get; set; }
        public virtual ICollection<Event> EventInformantTypeNavigation { get; set; }
        public virtual ICollection<BirthNotification> DeliveryTypeNavigation { get; set; }
        public virtual ICollection<BirthNotification> SkilledProfNavigation { get; set; }
        public virtual ICollection<Registrar> RelationshipNavigation { get; set; }
        public virtual ICollection<Witness> WitnessForNavigation {get; set; }

        public virtual ICollection<DeathEvent> DeathFacilityTypeNavigation { get; set; }
        public virtual ICollection<DeathEvent> DeathFacilityNavigation { get; set; }
        public virtual ICollection<DeathEvent> DeathPlaceOfDeathNavigation { get; set; }
        public virtual ICollection<DeathEvent> DuringDeathNavigation { get; set; }
        public virtual ICollection<DeathNotification> CauseOfDeathInfoTypeNavigation { get; set; }
        public virtual ICollection<MarriageEvent> MarriageTypeNavigation { get; set; }
        public virtual ICollection<Payment> PaymentNavigation { get; set; }
        public virtual ICollection<CourtCase> CourtsCaseCourtNavigation { get; set; }


        // public virtual ICollection<PaymentRate> PaymentRateNationNavigation { get; set; }
        [NotMapped]
        public string? ValueLang
        {
            get
            {
                return Value.Value<string>(lang);
            }
        }

    }
}