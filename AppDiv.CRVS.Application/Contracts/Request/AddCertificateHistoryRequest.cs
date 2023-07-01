using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddCertificateHistoryRequest
    {


        public JObject? Reason { get; set; }
        public string? SrialNo { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public Guid CerteficateId { get; set; }
        public Guid? ReasonLookupId { get; set; }
        public string? PrintType { get; set; }
    }
}