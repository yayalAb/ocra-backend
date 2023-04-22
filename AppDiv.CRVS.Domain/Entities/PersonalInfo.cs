
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities{
    public class PersonalInfo : BaseAuditableEntity{
        public string FirstNameStr { get; set; }
        public string MiddleNameStr { get; set; }
        public string? LastNameStr { get; set;}
        public DateTime? BirthDate { get; set; }
        public string NationalId { get; set; }
        public string SexLookupId { get; set; }
        public string? PlaceOfBirthLookupId { get; set; }
        public string NationalityLookupId { get; set; }
        public string? TitleLookupId { get; set; }
        public string? ReligionLookupId { get; set; }
        public string? EducationalStatusLookupId { get; set;}
        public string? TypeOfWorkLookupId { get; set;}
        public string MarriageStatusLookupId { get; set;}
        public string AddressId { get; set; }
        public string? NationLookupId { get; set; }
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

        public virtual Address Address { get; set; }
        public virtual Lookup SexLookup { get;set;}
        public virtual Lookup PlaceOfBirthLookup { get;set;}
        public virtual Lookup NationalityLookup { get;set;}
        public virtual Lookup TitleLookup { get; set;}
        public virtual Lookup ReligionLookup { get; set; }
        public virtual Lookup EducationalStatusLookup { get;set;}
        public virtual Lookup TypeOfWorkLookup { get;set;}
        public virtual Lookup MarraigeStatusLookup { get;set;}
        public virtual Lookup NationLookup { get;set;}
        public virtual ApplicationUser ApplicationUser { get;set;}





        
    }
}