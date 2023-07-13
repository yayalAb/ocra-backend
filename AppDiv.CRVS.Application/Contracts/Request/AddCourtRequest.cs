
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddCourtRequest
    {
        public Guid? Id { get; set; }
        public virtual Guid? AddressId { get; set; }
        public AddressResponseDTOE? AddressResponseDTO { get; set; }

        public JObject? Name { get; set; }
        public JObject? Description { get; set; }
    }
}