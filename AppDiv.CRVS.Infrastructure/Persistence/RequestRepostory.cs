using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class RequestRepostory : BaseRepository<Request>, IRequestRepostory
    {
        private readonly CRVSDbContext _DbContext;
        public RequestRepostory(CRVSDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
        }

        public async Task<IQueryable<Request>> GetAllRequest()
        {
            return _DbContext.Request
                        .Include(x => x.CivilRegOfficer)
                        .Include(x => x.AuthenticationRequest).
                         Include(c => c.AuthenticationRequest.Certificate)
                        .Include(x => x.CorrectionRequest)
                        .Include(w => w.Workflow).ThenInclude(ss => ss.Steps).AsQueryable();
        }


    }
}