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
    public class PlanRepository : BaseRepository<Plan>, IPlanRepository
    {
        private readonly CRVSDbContext _dbContext;

        public PlanRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Plan> GetPlans()
        {
            return _dbContext.Plans
                .Include(p => p.EventPlans)
                .Include(p => p.Address.ParentAddress.ParentAddress)
                .Include(p => p.ParentPlan)
                .ThenInclude(p => p.Address.ParentAddress.ParentAddress);
                
        }

        public IQueryable<EventPlan> GetEventPlans()
        {
            return _dbContext.EventPlans
                .Include(e => e.Plan);
        }
    }

}