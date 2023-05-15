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
        private readonly CRVSDbContext dbContext;

        public PersonalInfoRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PersonalInfo> GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

        public override async Task InsertAsync(PersonalInfo person, CancellationToken cancellationToken)
        {
            await base.InsertAsync(person, cancellationToken);


        }
        public void EFUpdate(PersonalInfo personalInfo)
        {
            dbContext.PersonalInfos.Update(personalInfo);
        }
        public void Attach(PersonalInfo personalInfo)
        {
            dbContext.PersonalInfos.Attach(personalInfo);
        }

        public PersonalInfo GetById(Guid id)
        {

            return dbContext.PersonalInfos.Find(id);
        }


    }
}