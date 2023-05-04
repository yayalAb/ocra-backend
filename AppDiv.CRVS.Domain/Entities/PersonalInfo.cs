
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class PersonalInfo : BaseAuditableEntity
    {
        public string FirstNameStr { get; set; }
        public string MiddleNameStr { get; set; }
        public string? LastNameStr { get; set; }
        public DateTime? BirthDate { get; set; }
        public string NationalId { get; set; }
        public Guid SexLookupId { get; set; }
        public Guid? PlaceOfBirthLookupId { get; set; }
        public Guid NationalityLookupId { get; set; }
        public Guid? TitleLookupId { get; set; }
        public Guid? ReligionLookupId { get; set; }
        public Guid? EducationalStatusLookupId { get; set; }
        public Guid? TypeOfWorkLookupId { get; set; }
        public Guid MarriageStatusLookupId { get; set; }
        public Guid AddressId { get; set; }
        public Guid? NationLookupId { get; set; }
        public Guid ContactInfoId { get; set; }
        [NotMapped]
        public JObject FirstName
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
        public JObject MiddleName
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
        public JObject LastName
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

        public virtual Address Address { get; set; }
        public virtual Lookup SexLookup { get; set; }
        public virtual Lookup PlaceOfBirthLookup { get; set; }
        public virtual Lookup NationalityLookup { get; set; }
        public virtual Lookup TitleLookup { get; set; }
        public virtual Lookup ReligionLookup { get; set; }
        public virtual Lookup EducationalStatusLookup { get; set; }
        public virtual Lookup TypeOfWorkLookup { get; set; }
        public virtual Lookup MarraigeStatusLookup { get; set; }
        public virtual Lookup NationLookup { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Event> EventCivilRegOfficers { get; set; }
        // public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ContactInfo ContactInfo { get; set; }
        public virtual BirthEvent BirthFather { get; set; }
        public virtual BirthEvent BirthMother { get; set; }
        public virtual Registrar RegistrarPersonalInfo { get; set; }

        public virtual DivorceEvent DivorceWife { get; set; }

        public AdoptionEvent AdoptiveMother { get; set; }
        public AdoptionEvent AdoptiveFather { get; set; }






    }
}