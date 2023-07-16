
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;


namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class MyReportRepository : BaseRepository<MyReports>, IMyReportRepository
    {
        private readonly CRVSDbContext _dbContext;
        public MyReportRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}