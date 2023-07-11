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
    public class RevocationTokenRepository : BaseRepository<RevocationToken>, IRevocationTokenRepository
    {
        public RevocationTokenRepository(CRVSDbContext dbContext) : base(dbContext)
        {
        }


    }
}


