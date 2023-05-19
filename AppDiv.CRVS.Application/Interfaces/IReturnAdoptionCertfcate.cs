using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IReturnAdoptionCertfcate
    {
        public AdoptionCertificateDTO GetAdoptionCertificate(AdoptionEvent adoption, string? BirthCertNo);

    }
}