using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IReturnBirthCertificate
    {
        public BirthCertificateDTO GetBirthCertificate(BirthEvent birth, string? BirthCertNo);

    }
}