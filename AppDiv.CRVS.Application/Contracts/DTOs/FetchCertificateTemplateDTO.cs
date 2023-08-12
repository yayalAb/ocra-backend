

using AppDiv.CRVS.Domain.Entities;
using Application.Common.Mappings;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public record FetchCertificateTemplateDTO : IMapFrom<CertificateTemplate>
    {
        public Guid Id { get; set; }
        public string CertificateType { get; set; }
        public string CreatedAt { get; set; }
        public string ModifiedAt { get; set; }
    }
}
