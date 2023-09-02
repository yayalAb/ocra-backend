

using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using Hangfire;

namespace AppDiv.CRVS.Infrastructure.Service.FireAndForgetJobs
{
    public interface IFireAndForgetJobs
    {
        public Task IndexPersonInfo(List<PersonalInfoEntry> entries);
        public Task IndexCertificate(List<CertificateEntry> certificateEntries);



    }
}