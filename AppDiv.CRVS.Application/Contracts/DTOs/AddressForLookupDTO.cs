using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AddressForLookupDTO
    {
        public Guid id { get; set; }
        public string AddressName { get; set; }
        public Guid? ParentAddressId { get; set; }
        public string? AdminType { get; set; }
    }
}