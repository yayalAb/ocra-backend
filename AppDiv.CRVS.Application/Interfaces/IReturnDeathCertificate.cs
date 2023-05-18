using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IReturnDeathCertificate
    {
        public DeathCertificateDTO GetDeathCertificate(DeathEvent death, string? BirthCertNo);

    }
}