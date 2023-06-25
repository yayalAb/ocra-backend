using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Domain.Entities
{
    public class MarriageApplication : BaseAuditableEntity
    {
        public DateTime ApplicationDate { get; set; }
        public string? ApplicationDateEt {get; set; }
        public Guid ApplicationAddressId { get; set; }
        public Guid BrideInfoId { get; set; }
        public Guid GroomInfoId { get; set;}
        public Guid CivilRegOfficerId { get; set; }
        public virtual Address ApplicationAddress { get; set;}
        public virtual PersonalInfo BrideInfo { get; set;}
        public virtual PersonalInfo GroomInfo { get; set;}
        public virtual PersonalInfo CivilRegOfficer { get; set;}
        public virtual MarriageEvent MarriageEvent { get; set; }

        [NotMapped]
        public string? _ApplicationDateEt
        {
            get { return ApplicationDateEt; }
            set
            {
                this.ApplicationDateEt = value;

                ApplicationDate = new CustomDateConverter(ApplicationDateEt).gorgorianDate;
            }
        }


    }
}