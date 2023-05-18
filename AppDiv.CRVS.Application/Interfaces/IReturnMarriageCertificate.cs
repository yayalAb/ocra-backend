using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IReturnMarriageCertificate
    {
        public MarriageCertificateDTO GetMarriageCertificate(MarriageEvent death, string? BirthCertNo);

    }
}