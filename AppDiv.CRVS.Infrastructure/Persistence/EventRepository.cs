using System.Linq.Expressions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        private readonly CRVSDbContext dbContext;

        public EventRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        async Task<Event> IEventRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
        public IQueryable<Event> GetAllQueryableAsync()
        {

            return dbContext.Events.AsQueryable();
        }

        public virtual async Task<bool> CheckForeignKey(Expression<Func<Event, bool>> where, Expression<Func<Event, object>> predicate)
        {
            return await this.dbContext.Events.Include(predicate).Where(where).FirstOrDefaultAsync() != null;

        }

        public async Task<Event?>? GetArchive(Guid id)
        {
            var eventType = this.GetAll().Where(e => e.Id == id).Select(e => e.EventType).FirstOrDefault();
            var e = this.GetAll().Where(e => e.Id == id)
                                .Include(m => m.EventAddress)
                                .Include(m => m.EventOwener)
                                .Include(m => m.EventOwener.NationalityLookup)
                                .Include(m => m.EventOwener.NationLookup)
                                .Include(m => m.EventOwener.ReligionLookup)
                                .Include(m => m.EventOwener.BirthAddress)
                                .Include(d => d.CivilRegOfficer);
            // var n =
            // return e.ElementAt(0);
            return await (eventType switch
            {
                "Birth" => e?.Include(e => e.BirthEvent)
                            .FirstOrDefaultAsync(),
                "Death" => e?.Include(e => e.DeathEventNavigation).FirstOrDefaultAsync(),
                "Adoption" => e?.Include(e => e.AdoptionEvent)
                                .Include(e => e.AdoptionEvent.AdoptiveFather)
                                .Include(e => e.AdoptionEvent.AdoptiveMother)
                                .Include(e => e.AdoptionEvent.CourtCase)
                                .Include(e => e.AdoptionEvent.CourtCase.Court)
                                .Include(e => e.AdoptionEvent.CourtCase.Court.Address)
                                .FirstOrDefaultAsync(),
                "Divorce" => e?.Include(e => e.DivorceEvent).FirstOrDefaultAsync(),
                "Marriage" => e?.Include(e => e.MarriageEvent).FirstOrDefaultAsync(),
                _ => null
            });
            // return await _dbContext.Certificates.Where(c => c.EventId == id).ToListAsync();

        }


    }
}