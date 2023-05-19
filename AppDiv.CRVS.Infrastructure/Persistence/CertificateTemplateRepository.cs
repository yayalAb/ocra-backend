
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;


namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CertificateTemplateRepository : BaseRepository<CertificateTemplate>, ICertificateTemplateRepository
    {
        private readonly CRVSDbContext _dbContext;

        public CertificateTemplateRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Add(CertificateTemplate certificateTemplate)
        {
            var cc = await _dbContext.CertificateTemplates.AddAsync(certificateTemplate);
            return cc.Entity.Id;
        }
        public IQueryable<CertificateTemplate> GetAllAsync()
        {

            return _dbContext.CertificateTemplates.AsQueryable();
        }

    }
}