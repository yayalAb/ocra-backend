

using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using Hangfire;

namespace AppDiv.CRVS.Infrastructure.Service.FireAndForgetJobs
{
    public interface IFireAndForgetJobs
    {
        public Task AddPersonIndex(List<PersonalInfoEntry> entries, string indexName);


    }
}