using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class CertificateHistory : BaseAuditableEntity
    {
        public string? ReasonStr { get; set; }
        public string SrialNo { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public Guid CerteficateId { get; set; }
        public string? PrintType { get; set; }
        public virtual PersonalInfo CivilRegOfficer { get; set; }
        public virtual Certificate Certeficate { get; set; }
        [NotMapped]
        public JObject? Reason
        {

            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ReasonStr) ? "{}" : ReasonStr);
            }
            set
            {
                ReasonStr = value.ToString();
            }
        }

    }
}