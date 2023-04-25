using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class ContactInfoRepository : BaseRepository<ContactInfo>, IContactInfoRepository
    {
        public ContactInfoRepository(CRVSDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ContactInfo> GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
        public override async Task InsertAsync(ContactInfo contact, CancellationToken cancellationToken)
        {
            await base.InsertAsync(contact, cancellationToken);
        }
    }
}