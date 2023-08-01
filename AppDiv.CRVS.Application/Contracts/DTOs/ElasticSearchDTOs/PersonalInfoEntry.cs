

using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs
{
    public class PersonalInfoEntry
    {
        public EntityState State { get; set; }
        public Guid PersonalInfoId { get; set; }
        public PersonalInfo? PersonalInfo { get; set; }
    }
}