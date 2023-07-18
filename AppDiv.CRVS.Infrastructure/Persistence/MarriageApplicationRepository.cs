using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class MarriageApplicationRepository : BaseRepository<MarriageApplication>, IMarriageApplicationRepository
    {
        private readonly CRVSDbContext dbContext;

        public MarriageApplicationRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public IQueryable<MarriageApplication> GetAllQueryableAsync()
        {

            return dbContext.MarriageApplications.AsQueryable();
        }
        public void EFUpdate(MarriageApplication marriageApplication)
        {
            dbContext.MarriageApplications.Update(marriageApplication);
        }
        public bool exists(Guid id)
        {
            return dbContext.MarriageApplications.Where(m => m.Id == id).Any();
        }

        public override async Task InsertAsync(MarriageApplication entity, CancellationToken cancellationToken)
        {
            entity.BrideInfo.SexLookupId = dbContext.Lookups.Where(l => l.Key == "sex")
                                        .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                                            || EF.Functions.Like(l.ValueStr, "%Dubara%")
                                            || EF.Functions.Like(l.ValueStr, "%Female%"))
                                        .Select(l => l.Id).FirstOrDefault();
            entity.GroomInfo.SexLookupId = dbContext.Lookups.Where(l => l.Key == "sex")
                                                .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                                                    || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                                                    || EF.Functions.Like(l.ValueStr, "%Male%"))
                                                .Select(l => l.Id).FirstOrDefault();

            await base.InsertAsync(entity, cancellationToken);

        }
        public IQueryable<MarriageApplication> GetAllApplications()
        {
            return dbContext.MarriageApplications.AsNoTracking()
                        .Include(a => a.BrideInfo)
                        .Include(a => a.GroomInfo)
                        .OrderByDescending(a => a.ApplicationDate)
                        .AsQueryable();
        }
    }
}