using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IReturnDivorceCertificate
    {
        public DivorceCertificateDTO GetDivorceCertificate(DivorceEvent divorce, string? BirthCertNo);

    }
}