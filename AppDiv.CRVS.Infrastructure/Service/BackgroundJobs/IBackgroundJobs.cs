using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public interface IBackgroundJobs
    {
        public Task job1();
        public Task SyncMarriageApplicationJob();
        public Task GetEventJob();
        public Task IndexCertificate(Certificate certificate);
        public Task RemoveCertificate(Guid certificateId);
        public Task updateCertificate(Certificate certificate);
    }
}