
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs
{
    public class CertificateEntry
    {
        public EntityState State { get; set; }
        public Guid CertificateId {get; set; }
        public Certificate? Certificate { get; set; }
    }
}