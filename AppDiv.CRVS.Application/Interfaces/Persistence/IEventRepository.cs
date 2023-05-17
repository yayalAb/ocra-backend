using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IEventRepository : IBaseRepository<Event>
    {

        public IQueryable<Event> GetAllQueryableAsync();

        Task<Event> GetByIdAsync(Guid id);
        Task<bool> CheckForeignKey(Expression<Func<Event, bool>> where, Expression<Func<Event, object>> predicate);
    }
}