using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IPlanRepository : IBaseRepository<Plan>
    {
        IQueryable<Plan> GetPlans();
        IQueryable<EventPlan> GetEventPlans();
    }
}