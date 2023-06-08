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

        private Task<Event?> BirthIncludes(IQueryable<Event> birth)
        {
            return birth.Include(e => e.BirthEvent)
                            .Include(m => m.EventOwener.ResidentAddress)
                            .Include(m => m.EventOwener.BirthAddress)
                            .Include(e => e.BirthEvent.BirthPlace)
                            .Include(e => e.BirthEvent.TypeOfBirthLookup)
                            .Include(e => e.BirthEvent.BirthNotification)
                            .Include(e => e.BirthEvent.BirthNotification.DeliveryTypeLookup)
                            .Include(e => e.BirthEvent.BirthNotification.SkilledProfLookup)
                            .Include(e => e.BirthEvent.Mother)
                            .Include(e => e.BirthEvent.Father)
                            .Include(e => e.EventRegistrar.RegistrarInfo)
                            .Include(e => e.EventRegistrar.RegistrarInfo.BirthAddress)
                            .Include(e => e.EventRegistrar.RegistrarInfo.ResidentAddress)
                            .Include(e => e.EventRegistrar.RegistrarInfo.SexLookup)
                            .Include(e => e.EventRegistrar.RelationshipLookup)
                            .FirstOrDefaultAsync();
        }
        private Task<Event?> DeathIncludes(IQueryable<Event> death)
        {
            return death.Include(e => e.DeathEventNavigation)
                                .Include(m => m.EventOwener.BirthAddress)
                                .Include(m => m.EventOwener.ResidentAddress)
                                .Include(e => e.DeathEventNavigation.DeathNotification)
                                .Include(e => e.DeathEventNavigation.DeathNotification.CauseOfDeathInfoTypeLookup)
                                .Include(e => e.DeathEventNavigation.DuringDeathLookup)
                                .Include(e => e.EventRegistrar.RegistrarInfo)
                                .Include(e => e.EventRegistrar.RelationshipLookup)
                                .FirstOrDefaultAsync();
        }
        private Task<Event?> AdoptionIncludes(IQueryable<Event> adoption)
        {
            return adoption.Include(e => e.AdoptionEvent)
                                .Include(m => m.EventOwener.BirthAddress)
                                .Include(m => m.EventOwener.ResidentAddress)
                                .Include(e => e.AdoptionEvent.AdoptiveFather)
                                .Include(e => e.AdoptionEvent.AdoptiveFather.ResidentAddress)
                                .Include(e => e.AdoptionEvent.AdoptiveFather.BirthAddress)
                                .Include(e => e.AdoptionEvent.AdoptiveFather.MarraigeStatusLookup)
                                .Include(e => e.AdoptionEvent.AdoptiveFather.TypeOfWorkLookup)
                                .Include(e => e.AdoptionEvent.AdoptiveFather.NationalityLookup)
                                .Include(e => e.AdoptionEvent.AdoptiveFather.EducationalStatusLookup)
                                .Include(e => e.AdoptionEvent.AdoptiveFather.NationLookup)
                                .Include(e => e.AdoptionEvent.AdoptiveMother)
                                .Include(e => e.AdoptionEvent.AdoptiveMother.ResidentAddress)
                                .Include(e => e.AdoptionEvent.AdoptiveMother.BirthAddress)
                                .Include(e => e.AdoptionEvent.AdoptiveMother.MarraigeStatusLookup)
                                .Include(e => e.AdoptionEvent.AdoptiveMother.TypeOfWorkLookup)
                                .Include(e => e.AdoptionEvent.AdoptiveMother.NationalityLookup)
                                .Include(e => e.AdoptionEvent.AdoptiveMother.EducationalStatusLookup)
                                .Include(e => e.AdoptionEvent.AdoptiveMother.NationLookup)
                                .Include(e => e.AdoptionEvent.CourtCase)
                                .Include(e => e.AdoptionEvent.CourtCase.Court)
                                .Include(e => e.AdoptionEvent.CourtCase.Court.Address)
                                .FirstOrDefaultAsync();
        }
        private Task<Event?> MarriageIncludes(IQueryable<Event> marriage)
        {
            return marriage.Include(e => e.MarriageEvent)
                            .Include(m => m.EventOwener.BirthAddress)
                            .Include(m => m.EventOwener.ResidentAddress)
                            .Include(m => m.EventOwener.EducationalStatusLookup)
                            .Include(e => e.MarriageEvent.BrideInfo)
                            .Include(e => e.MarriageEvent.BrideInfo.ResidentAddress)
                            .Include(e => e.MarriageEvent.BrideInfo.ReligionLookup)
                            .Include(e => e.MarriageEvent.BrideInfo.BirthAddress)
                            .Include(e => e.MarriageEvent.BrideInfo.MarraigeStatusLookup)
                            .Include(e => e.MarriageEvent.BrideInfo.TypeOfWorkLookup)
                            .Include(e => e.MarriageEvent.BrideInfo.NationalityLookup)
                            .Include(e => e.MarriageEvent.BrideInfo.ReligionLookup)
                            .Include(e => e.MarriageEvent.BrideInfo.EducationalStatusLookup)
                            .Include(e => e.MarriageEvent.BrideInfo.NationLookup)
                            .Include(e => e.MarriageEvent.MarriageType)
                            .Include(e => e.MarriageEvent.Witnesses)
                            .ThenInclude(w => w.WitnessPersonalInfo).ThenInclude(p => p.ResidentAddress)
                            .FirstOrDefaultAsync();
        }
        private Task<Event?> DivorceIncludes(IQueryable<Event> divorce)
        {
            return divorce.Include(e => e.DivorceEvent)
                        .Include(e => e.DivorceEvent.DivorcedWife)
                        .Include(e => e.DivorceEvent.DivorcedWife.ResidentAddress)
                        .Include(e => e.DivorceEvent.DivorcedWife.BirthAddress)
                        .Include(e => e.DivorceEvent.DivorcedWife.MarraigeStatusLookup)
                        .Include(e => e.DivorceEvent.DivorcedWife.TypeOfWorkLookup)
                        .Include(e => e.DivorceEvent.DivorcedWife.NationalityLookup)
                        .Include(e => e.DivorceEvent.DivorcedWife.EducationalStatusLookup)
                        .Include(e => e.DivorceEvent.DivorcedWife.NationLookup)
                        .Include(e => e.DivorceEvent.CourtCase)
                        .Include(e => e.DivorceEvent.CourtCase.Court)
                        .Include(e => e.DivorceEvent.CourtCase.Court.Address)
                        .FirstOrDefaultAsync();
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
                                .Include(m => m.EventOwener.SexLookup)
                                .Include(m => m.PaymentExamption)
                                .Include(d => d.CivilRegOfficer);
            // var n =
            // return e.ElementAt(0);
            return await (eventType switch
            {
                "Birth" => BirthIncludes(e),
                "Death" => DeathIncludes(e),
                "Adoption" => AdoptionIncludes(e),
                "Divorce" => DivorceIncludes(e),
                "Marriage" => MarriageIncludes(e),
                _ => null
            });
            // return await _dbContext.Certificates.Where(c => c.EventId == id).ToListAsync();
        }
    }
}