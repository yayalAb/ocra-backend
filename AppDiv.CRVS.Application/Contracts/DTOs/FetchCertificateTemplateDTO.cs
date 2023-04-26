

using AppDiv.CRVS.Domain.Entities;
using Application.Common.Mappings;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public record FetchCertificateTemplateDTO : IMapFrom<CertificateTemplate>
    {
        public Guid Id { get; set; }
        public string CertificateType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
