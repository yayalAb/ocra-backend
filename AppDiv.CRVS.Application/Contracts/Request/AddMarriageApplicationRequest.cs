using AppDiv.CRVS.Application.Contracts.DTOs;
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
        public DateTime CreatedDate { get; set; }
    }
}

