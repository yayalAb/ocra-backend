using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Persistence.Couch;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Infrastructure.Context;
using AppDiv.CRVS.Infrastructure.Persistence.Couch;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Infrastructure.CouchModels;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class LookupRepository : BaseRepository<Lookup>, ILookupRepository
    {
        private readonly CRVSDbContext dbContext;
        private readonly ILookupCouchRepository lookupCouchRepo;

        public LookupRepository(CRVSDbContext dbContext, ILookupCouchRepository lookupCouchRepo) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.lookupCouchRepo = lookupCouchRepo;
        }

        public async Task<Lookup> GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
        public async Task Import(ICollection<Lookup> lookups, CancellationToken cancellationToken)
        {
            await dbContext.Lookups.AddRangeAsync(lookups, cancellationToken);
        }

        public Task<Lookup> GetLookupByKey(string key)
        {
            return base.GetFirstEntryAsync(x => x.Key.Equals(key), q => q.Id, Utility.Contracts.SortingDirection.Ascending);
        }

        public async Task<Lookup?> GetLookupById(Guid id)
        {
            return await dbContext.Lookups.FindAsync(id);
        }

        // public bool Exists(Guid id)
        // {
        //     return dbContext.Lookups.Where(p => p.Id == id).Any();
        // }
        public virtual async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {

            var entries = dbContext.ChangeTracker
              .Entries()
              .Where(e => e.Entity is Lookup &&
                      (e.State == EntityState.Added
                      || e.State == EntityState.Modified || e.State == EntityState.Deleted));
            List<LookupEntry> lookupEntries = entries.Select(e => new LookupEntry
            {
                State = e.State,
                Lookup = CustomMapper.Mapper.Map<LookupCouchDTO>((Lookup)e.Entity)
            }).ToList();

            bool saveRes = await base.SaveChangesAsync(cancellationToken);

            if (saveRes)
            {
                foreach (var entry in lookupEntries)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            await lookupCouchRepo.InsertLookupAsync(entry.Lookup);
                            break;
                        case EntityState.Modified:
                            await lookupCouchRepo.UpdateLookupAsync(entry.Lookup);
                            break;
                        case EntityState.Deleted:
                            await lookupCouchRepo.RemoveLookupAsync(entry.Lookup);
                            break;
                        default: break;

                    }
                }

            }
            return saveRes;


        }
        public async Task InitializeLookupCouch()
        {
            var empty = await lookupCouchRepo.IsEmpty();
            if (empty)
            {
                await lookupCouchRepo.BulkInsertAsync(dbContext.Lookups.ToList());
            }
        }
    }
}