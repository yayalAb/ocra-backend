using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public interface IBackgroundJobs
    {
         public  Task job1();
         public Task SyncMarriageApplicationJob();
         public  Task GetEventJob();
    }
}