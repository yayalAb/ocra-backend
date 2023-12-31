
using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class AuthenticationRequest : BaseAuditableEntity
    {
        public Guid CertificateId { get; set; }
        public Guid RequestId { get; set; }
        public virtual Certificate Certificate { get; set; }
        public virtual Request Request { get; set; }
    }
}