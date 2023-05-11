using AppDiv.CRVS.Application.Contracts.Request;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class CourtDTO
    {
        public Guid id { get; set; }
        public virtual AddAddressRequest Address { get; set; }
        public JObject Name { get; set; }
        public JObject Description { get; set; }
    }
}