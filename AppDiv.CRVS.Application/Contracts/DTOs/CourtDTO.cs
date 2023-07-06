using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class CourtDTO
    {
        public Guid id { get; set; }
        public Guid AddressId { get; set; }
        public JObject Name { get; set; }
        public JObject Description { get; set; }
        public AddressResponseDTOE? CourtAddress { get; set; }

    }
}