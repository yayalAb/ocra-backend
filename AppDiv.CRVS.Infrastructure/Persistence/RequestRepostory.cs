using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class RequestRepostory : BaseRepository<Request>, IRequestRepostory
    {
        private readonly CRVSDbContext _dbContext;
        public RequestRepostory(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
         public IQueryable<Request> GetAllQueryableAsync()
        {

            return _dbContext.Requests.AsQueryable();
        }

        public async Task<IQueryable<Request>> GetAllRequest()
        {
            return _dbContext.Requests
                        .Include(x => x.CivilRegOfficer)
                        .Include(x => x.AuthenticationRequest).
                         Include(c => c.AuthenticationRequest.Certificate)
                        .Include(x => x.CorrectionRequest)
                        .Include(w => w.Workflow).ThenInclude(ss => ss.Steps).AsQueryable();
        }
        


    }
}