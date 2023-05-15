using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IPersonalInfoRepository : IBaseRepository<PersonalInfo>
    {
        Task<IEnumerable<PersonalInfo>> GetAllAsync();
        Task<PersonalInfo> GetByIdAsync(Guid id);
        public void EFUpdate(PersonalInfo personalInfo);
        public void Attach(PersonalInfo personalInfo);
        public PersonalInfo GetById(Guid id);
    }
}