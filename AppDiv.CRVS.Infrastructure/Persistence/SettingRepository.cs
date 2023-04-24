
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
    public class SettingRepository : BaseRepository<Setting>, ISettingRepository
    {
        public SettingRepository(CRVSDbContext dbContext) : base(dbContext)
        {
        }

        async Task<Setting> ISettingRepository.GetAddressByKey(string key)
        {
            return await base.GetAsync(key);
        }
        Task<Setting> ISettingRepository.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}