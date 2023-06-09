using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Features.Search;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IPersonalInfoRepository : IBaseRepository<PersonalInfo>
    {
        Task<IEnumerable<PersonalInfo>> GetAllAsync();
        Task<PersonalInfo> GetByIdAsync(Guid id);
        Task<List<PersonSearchResponse>> SearchSimilarPersons(SearchSimilarPersonalInfoQuery queryParams);
        public  Task<List<PersonSearchResponse>> SearchPersonalInfo(GetPersonalInfoQuery query);
        
        public void EFUpdate(PersonalInfo personalInfo);
        public void Attach(PersonalInfo personalInfo);
        public PersonalInfo GetById(Guid id);
        public bool CheckPerson(Guid id);
        public bool Exists(Guid id);
         public IQueryable<PersonalInfo> GetAllQueryable();

    }
}