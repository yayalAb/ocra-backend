
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
using AppDiv.CRVS.Application.Interfaces.Persistence.Couch;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Infrastructure.CouchModels;
using AppDiv.CRVS.Application.Mapper;
using  AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class SettingRepository : BaseRepository<Setting>, ISettingRepository
    {
        private readonly CRVSDbContext dbContext ;
        private readonly ISettingCouchRepository settingCouchRepo;
        public SettingRepository(CRVSDbContext dbContext , ISettingCouchRepository settingCouchRepo) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.settingCouchRepo= settingCouchRepo;    
        }

        async Task<Setting> ISettingRepository.GetSettingByKey(string key)
        {
            return await base.GetAsync(key);
        }
        async Task<Setting> ISettingRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

        public virtual async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {

            var entries = dbContext.ChangeTracker
              .Entries()
              .Where(e => e.Entity is Setting &&
                      (e.State == EntityState.Added
                      || e.State == EntityState.Modified || e.State == EntityState.Deleted));
            List<SettingEntry> settingEntries = entries.Select(e => new SettingEntry
            {
                State = e.State,
                Setting = CustomMapper.Mapper.Map<SettingDTO>((Setting)e.Entity)
            }).ToList();

            bool saveRes = await base.SaveChangesAsync(cancellationToken);

            if (saveRes)
            {
                foreach (var entry in settingEntries)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            await settingCouchRepo.InsertSettingAsync(entry.Setting);
                            break;
                        case EntityState.Modified:
                            await settingCouchRepo.UpdateSettingAsync(entry.Setting);
                            break;
                        case EntityState.Deleted:
                            await settingCouchRepo.RemoveSettingAsync(entry.Setting);
                            break;
                        default: break;

                    }
                }

            }
            return saveRes;


        }
        public async Task InitializeSettingCouch()
        {
            var empty = await settingCouchRepo.IsEmpty();
            if (empty)
            {
                await settingCouchRepo.BulkInsertAsync(dbContext.Settings);
            }
        }
    }
}