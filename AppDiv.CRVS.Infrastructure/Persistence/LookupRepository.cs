using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Infrastructure.Context;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class LookupRepository : BaseRepository<Lookup>, ILookupRepository
    {
        private readonly CRVSDbContext dbContext;

        public LookupRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Lookup> GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

        public Task<Lookup> GetLookupByKey(string key)
        {
            return base.GetFirstEntryAsync(x => x.Key.Equals(key), q => q.Id, Utility.Contracts.SortingDirection.Ascending);
        }

        public Lookup? GetLookupById(Guid id){
            return dbContext.Lookups.Find(id);
        }
    }
}