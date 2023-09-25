using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddAddressRequest
    {
        public Guid? Id { get; set; }
        public JObject AddressName { get; set; }
        public string StatisticCode { get; set; }
        public string Code { get; set; }
        public string? CodePerfix { get; set; }
        public string? CodePostfix { get; set; }
        public int AdminLevel { get; set; } = 1;
        public Guid? AreaTypeLookupId { get; set; }
        public Guid? ParentAddressId { get; set; }
        public Guid? AdminTypeLookupId { get; set; }
        public DateTime? WorkStartedOn {get;set;}
    }
}