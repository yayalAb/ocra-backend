using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddMarriageApplicationRequest
    {
        public Guid Id { get; set; }
        public string ApplicationDateEt { get; set; }
        public Guid? ApplicationAddressId { get; set; }
        public BrideInfoDTO BrideInfo { get; set; }
        public GroomInfoDTO GroomInfo { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public string? CreatedDate { get; set; }
        public DateTime? CreatedAt { get; set; }

        [NotMapped]
        public string? _CreatedDate
        {
            get { return CreatedDate; }
            set
            {
                // this.CreatedDate = value;

                CreatedAt = string.IsNullOrEmpty(CreatedDate) ? null : new CustomDateConverter(CreatedDate).gorgorianDate;
            }
        }

    }
}

