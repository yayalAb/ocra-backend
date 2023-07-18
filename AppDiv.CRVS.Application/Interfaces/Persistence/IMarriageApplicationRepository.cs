using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IMarriageApplicationRepository : IBaseRepository<MarriageApplication>
    {
        public IQueryable<MarriageApplication> GetAllQueryableAsync();
        public void EFUpdate(MarriageApplication marriageApplication);
        public bool exists(Guid id);
        new public Task InsertAsync(MarriageApplication entity, CancellationToken cancellationToken);
        public IQueryable<MarriageApplication> GetAllApplications();
    }
}