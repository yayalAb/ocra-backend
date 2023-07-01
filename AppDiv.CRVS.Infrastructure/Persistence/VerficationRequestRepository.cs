using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class VerficationRequestRepository : BaseRepository<VerficationRequest>, IVerficationRequestRepository
    {
        private readonly CRVSDbContext _DbContext;
        public VerficationRequestRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
        }
        async Task<VerficationRequest> IVerficationRequestRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }


    }
}