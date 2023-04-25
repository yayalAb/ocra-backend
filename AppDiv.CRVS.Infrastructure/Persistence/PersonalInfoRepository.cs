using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class PersonalInfoRepository : BaseRepository<PersonalInfo>, IPersonalInfoRepository
    {
        public PersonalInfoRepository(CRVSDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<PersonalInfo> GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

        public override async Task InsertAsync(PersonalInfo person, CancellationToken cancellationToken)
        {
            await base.InsertAsync(person, cancellationToken);


        }
    }
}