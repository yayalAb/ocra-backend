using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AddressForMergeDTO
    {
        public Guid Id { get; set; }
        public JObject? AddressName { get; set; }
        public Guid? ParentAddressId { get; set; }
        public Guid? AdminTypeLookupId { get; set; }
        public int? AdminLevel { get; set; }
        public string? StatisticCode { get; set; }
        public string? Code { get; set; }
        public string? CodePrefix { get; set; }
        public string? CodePostfix { get; set; }

    }
}
