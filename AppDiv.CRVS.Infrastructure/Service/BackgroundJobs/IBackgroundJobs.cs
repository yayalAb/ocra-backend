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
        public Task SyncMarriageApplicationJob();
        public Task GetEventJob();
        public Task SyncCertificatesAndPayments();
    }
}