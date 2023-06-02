using System.Linq.Expressions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        private readonly CRVSDbContext dbContext;
        private readonly IElasticClient _elasticClient;

        public EventRepository(CRVSDbContext dbContext, IElasticClient elasticClient) : base(dbContext)
        {
            this.dbContext = dbContext;
            _elasticClient = elasticClient;
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
        public async Task elasticSearchDemo()
        {
            if (!_elasticClient.Indices.Exists("person2").Exists)
            {

                _elasticClient.Indices.Create("person2", index => index.Map<PersonDtoElastic>(x => x.AutoMap()));
            }
            // await _elasticClient.IndexDocumentAsync(new PersonDtoElastic
            // {
            //     FirstNameStr = "name",
            //     MiddleNameStr = "name2",
            //     LastNameStr = "name3",
            //     NationalId = "kkkk",
            //     BirthDateEt = "bbbb"
            // });
        var response1 =     _elasticClient
                .IndexMany<PersonDtoElastic>(dbContext.PersonalInfos
                    .Select(p => new PersonDtoElastic
                    {
                        FirstNameStr = p.FirstNameStr,
                        MiddleNameStr = p.MiddleNameStr,
                        LastNameStr = p.LastNameStr,
                        NationalId = p.NationalId,
                        BirthDateEt = p.BirthDateEt
                    }), "person2");
            var response = _elasticClient
              .SearchAsync<PersonDtoElastic>(s =>
              s.Query(q => q.QueryString(d => d.Query('*' + "kdjfakl" + '*')))
              .Size(5000));
            var result = response.Result.Documents.ToList();
            var res = await _elasticClient.GetAsync<PersonDtoElastic>(1, idx => idx.Index("person2"));
            var res2 =   await _elasticClient.SearchAsync<PersonDtoElastic>(s => s
                                        .Query(q => q
                                         
                                            .Match(m => m
                                                .Field(f => f.NationalId)
                                                .Query("string")
                                                
                                            )
                                        )
);
//             var res3 =   await _elasticClient.SearchAsync<PersonDtoElastic>(s => s
//                                         .Query(q => q.
//                                         )
// );
         if(!res2.IsValid){
           var error =  res2.OriginalException?.Message;
         }
            var f = true;

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