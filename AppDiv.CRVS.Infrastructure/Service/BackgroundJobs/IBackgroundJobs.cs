using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public interface IBackgroundJobs
    {
        public Task job1();
        public Task SyncMarriageApplicationJob();
        public Task GetEventJob();
        // public Task AddIndex<T>(List<object> entities, string indexName) where T : BaseIndex;
        // public Task RemoveIndex<T>(List<Guid> indexIds, string indexName) where T : BaseIndex;

        // public Task UpdateIndex<T>(List<object> entities, string indexName) where T : BaseIndex;
        public Task AddPersonIndex(List<PersonalInfoIndex> entities, string indexName);
    }
}