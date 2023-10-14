using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class LoginHistoryRepository : BaseRepository<LoginHistory>, ILoginHistoryRepository
    {
        private readonly CRVSDbContext _dbContext;

        public LoginHistoryRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<LoginHistory> GetAllGrid()
        {
            return _dbContext.LoginHistorys
                                .AsNoTracking()
                                .Include(l => l.User.PersonalInfo)
                                .Include(u => u.User.Address.ParentAddress.ParentAddress);
        }

    }
}

