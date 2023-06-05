
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Request : BaseAuditableEntity
    {
        public string RequestType { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public int currentStep { get; set; }
        public virtual PersonalInfo CivilRegOfficer { get; set; }
        public virtual CorrectionRequest CorrectionRequest { get; set; }
        public virtual AuthenticationRequest AuthenticationRequest { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

    }
}