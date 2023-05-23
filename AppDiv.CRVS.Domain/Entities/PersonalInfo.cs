
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EthiopianCalendar;


namespace AppDiv.CRVS.Domain.Entities
{
    public class PersonalInfo : BaseAuditableEntity
    {
        public string FirstNameStr { get; set; }
        public string? MiddleNameStr { get; set; }
        public string? LastNameStr { get; set; }
        public string? NationalId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? BirthDateEt { get; set; }
        public Guid SexLookupId { get; set; }
        public Guid? PlaceOfBirthLookupId { get; set; }
        public Guid? NationalityLookupId { get; set; }
        public Guid? TitleLookupId { get; set; }
        public Guid? ReligionLookupId { get; set; }
        public Guid? EducationalStatusLookupId { get; set; }
        public Guid? TypeOfWorkLookupId { get; set; }
        public Guid? MarriageStatusLookupId { get; set; }
        public Guid? BirthAddressId { get; set; }
        public Guid? ResidentAddressId { get; set; }
        public Guid? NationLookupId { get; set; }
        public Guid? ContactInfoId { get; set; }
        [NotMapped]
        public string? _BirthDateEt
        {
            get { return BirthDateEt; }
            set
            {
                BirthDateEt = value;
                BirthDate = new EthiopianDate(DateTime.Parse(BirthDateEt).Year, DateTime.Parse(BirthDateEt).Month, DateTime.Parse(BirthDateEt).Day).ToGregorianDate();
            }
        }
        [NotMapped]
        public JObject? FirstName
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(FirstNameStr) ? "{}" : FirstNameStr);
            }
            set
            {
                FirstNameStr = value.ToString();
            }
        }

        [NotMapped]
        public string? FirstNameLang
        {
            get
            {
                return FirstName.Value<string>(lang);
            }
        }
        [NotMapped]
        public JObject? MiddleName
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(MiddleNameStr) ? "{}" : MiddleNameStr);
            }
            set
            {
                MiddleNameStr = value.ToString();
            }
        }
        [NotMapped]
        public string? MiddleNameLang
        {
            get
            {
                return MiddleName.Value<string>(lang);
            }
        }
        [NotMapped]
        public JObject? LastName
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(LastNameStr) ? "{}" : LastNameStr);
            }
            set
            {
                LastNameStr = value.ToString();
            }
        }
        [NotMapped]
        public string? LastNameLang
        {
            get
            {
                return LastName.Value<string>(lang);
            }
        }

        public virtual Address BirthAddress { get; set; }
        public virtual Address ResidentAddress { get; set; }
        public virtual Lookup SexLookup { get; set; }
        public virtual Lookup PlaceOfBirthLookup { get; set; }
        public virtual Lookup NationalityLookup { get; set; }
        public virtual Lookup TitleLookup { get; set; }
        public virtual Lookup ReligionLookup { get; set; }
        public virtual Lookup EducationalStatusLookup { get; set; }
        public virtual Lookup TypeOfWorkLookup { get; set; }
        public virtual Lookup MarraigeStatusLookup { get; set; }
        public virtual Lookup NationLookup { get; set; }
        public virtual ContactInfo ContactInfo { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Event> EventCivilRegOfficers { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<BirthEvent> BirthFatherNavigation { get; set; }
        public virtual ICollection<BirthEvent> BirthMotherNavigation { get; set; }
        public virtual ICollection<Registrar> RegistrarPersonalInfoNavigation { get; set; }

        public virtual ICollection<DivorceEvent> DivorceWifeNavigation { get; set; }

        public ICollection<AdoptionEvent> AdoptiveMotherNavigation { get; set; }
        public ICollection<AdoptionEvent> AdoptiveFatherNavigation { get; set; }
        public virtual ICollection<Witness> Witness { get; set; }
        public virtual ICollection<MarriageEvent> MarriageEventBrideInfo { get; set; }
        public virtual ICollection<MarriageApplication> MarriageApplicationBrideInfo { get; set; }
        public virtual ICollection<MarriageApplication> MarriageApplicationGroomInfo { get; set; }
        public virtual ICollection<MarriageApplication> MarriageApplicationCivilRegOfficer { get; set; }

    }
}