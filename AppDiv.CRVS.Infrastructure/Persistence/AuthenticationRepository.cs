using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class AuthenticationRepository : BaseRepository<AuthenticationRequest>, IAuthenticationRepository
    {
        private readonly CRVSDbContext _DbContext;
        public AuthenticationRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
        }
        async Task<AuthenticationRequest> IAuthenticationRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }


    }
}