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
        // public async Task<Event?> GetContent(Guid eventId)
        // {
        //     var eventType = base.GetAll().Where(e => e.Id == eventId).Select(e => e.EventType).FirstOrDefault();
        //     switch (eventType)
        //     {
        //         case "BirthEvent":
        //             return await base.GetAll()
        //                     // .Include(d => d.Father)
        //                     // .Include(d => d.Mother)
        //                     // .Include(d => d.Event).ThenInclude(e => e.EventOwener)
        //                     // .Include(d => d.Event).ThenInclude(e => e.CivilRegOfficer)
        //                     // .Include(d => d.Event).ThenInclude(e => e.EventRegistrar)
        //                     .Where(e => e.Id == eventId).FirstOrDefaultAsync()
        //                     ;
        //         case "DeathEvent":
        //             return await base.GetAll()
        //                     // .Include(d => d.Event).ThenInclude(e => e.EventOwener)
        //                     // .ThenInclude(p => p.TitleLookup).ThenInclude(p => p.SexLookup).
        //                     // .Include(d => d.Event).ThenInclude(e => e.EventOwener)
        //                     .Where(e => e.Id == eventId).FirstOrDefaultAsync()
        //                     ;
        //         case "Adoption":
        //             return
        //                     await base.GetAll()
        //                     .Where(e => e.Id == eventId).FirstOrDefaultAsync()
        //                     // _dbContext.AdoptionEvents
        //                     // .Include(d => d.Event).ThenInclude(d => d.PaymentExamption)
        //                     // // .Include(d => d.Event).ThenInclude(e => e.EventOwener)
        //                     // .Include(d => d.AdoptiveFather)
        //                     // .Include(d => d.AdoptiveMother)
        //                     // .Include(d => d.CourtCase)
        //                     ;
        //         case "Marriage":
        //             return
        //                     await base.GetAll()
        //                         .Where(m => m.Id == eventId).FirstOrDefaultAsync(), null);
        //         // _dbContext.MarriageEvents
        //         //     .Include(m => m.BrideInfo)
        //         //     .ThenInclude(b => b.ContactInfo)
        //         //     .Include(m => m.Event)
        //         //     .Include(m => m.Event.EventOwener).ThenInclude(e => e.ContactInfo)
        //         //     .Include(m => m.Event.EventSupportingDocuments)
        //         //     .Include(m => m.Witnesses)
        //         //     .Include(m => m.Event.PaymentExamption).ThenInclude(p => p.SupportingDocuments)
        //         case "Divorce":
        //             return
        //                     await base.GetAll()
        //                         // _dbContext.DivorceEvents
        //                         //     .Include(m => m.CourtCase)
        //                         //     .Include(m => m.DivorcedWife)
        //                         //     .ThenInclude(b => b.ContactInfo)
        //                         //     .Include(m => m.Event)
        //                         //     .Include(m => m.Event.EventOwener).ThenInclude(e => e.ContactInfo)
        //                         //     .Include(m => m.Event.EventSupportingDocuments)
        //                         //     .Include(m => m.Event.PaymentExamption).ThenInclude(p => p.SupportingDocuments)
        //                         .Where(m => m.Id == eventId).FirstOrDefaultAsync());
        //     }
        //     return null;
        // }

    }
}