
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Address : BaseAuditableEntity
    {
        public string AddressNameStr { get; set; }
        public string StatisticCode { get; set; }
        public string Code { get; set; }
        public int AdminLevel { get; set; } = 1;
        public Guid? AreaTypeLookupId { get; set; }
        public Guid? ParentAddressId { get; set; }
        [NotMapped]
        public JObject AddressName
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(AddressNameStr) ? "{}" : AddressNameStr);
            }
            set
            {
                AddressNameStr = value.ToString();
            }
        }
        // public virtual Lookup AdminLevelLookup { get; set; }
        public virtual Lookup AreaTypeLookup { get; set; }
        public virtual Address ParentAddress { get; set; }
        public virtual ICollection<Event> EventAddresses { get; set; }
        public virtual ICollection<Address> ChildAddresses { get; set; }
        public virtual ICollection<PersonalInfo> PersonalInfoBirthAddresses { get; }
        public virtual ICollection<PersonalInfo> PersonalInfoResidentAddresses { get; }

        // public virtual ICollection<BirthEvent> AddressBirthPlaceNavigation { get; set; }
        public virtual ICollection<AdoptionEvent> BeforeAdoptionAddressNavigation { get; set; }
        public virtual ICollection<MarriageApplication> MarriageApplications {get; set;}
        public virtual ICollection<ApplicationUser> ApplicationuserAddresses { get; set;}
        public virtual ICollection<PaymentExamptionRequest> ExamptionRequestAddresses { get; set; }

        [NotMapped]
        public string? AddressNameLang
        {
            get
            {
                return AddressName.Value<string>(lang);
            }
        }

    }
}