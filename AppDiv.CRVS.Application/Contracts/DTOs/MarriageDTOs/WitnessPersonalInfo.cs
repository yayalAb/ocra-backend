
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class WitnessInfoDTO
    {
        public Guid? Id { get; set; } = null;
        public JObject FirstName { get; set; }
        public JObject MiddleName { get; set; }
        public JObject LastName { get; set; }
        public Guid? SexLookupId { get; set; }
        // public DateTime? BirthDate { get; set; }
        // public Guid NationalityLookupId {get; set; }
        public string? NationalId { get; set; }
        public Guid? ResidentAddressId { get; set; }
        public string? PhoneNumber { get; set; }
        public AddressResponseDTOE? ResidentAddressResponseDTO { get; set; }

        public DateTime? CreatedAt {get; set; }
        public Guid? CreatedBy {get; set; }


    }
}