using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AddressDTO
    {
        public Guid id { get; set; }
        public JObject AddressName { get; set; }
        public string StatisticCode { get; set; }
        public string Code { get; set; }
        public int AdminLevel { get; set; }
        public Guid AreaTypeLookupId { get; set; }
        public Guid? ParentAddressId { get; set; }
        public AddressDTO ParentAddress { get; set; }
    }
}