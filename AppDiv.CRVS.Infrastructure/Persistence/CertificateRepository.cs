using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Features.Search;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.CouchModels;
using AppDiv.CRVS.Infrastructure.Service;
using AppDiv.CRVS.Infrastructure.Service.FireAndForgetJobs;
using AppDiv.CRVS.Infrastructure.Services;
using CouchDB.Driver.Query.Extensions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Nest;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CertificateRepository : BaseRepository<Certificate>, ICertificateRepository
    {
        private readonly CRVSDbContext _dbContext;
        private readonly IElasticClient _elasticClient;
        private readonly IUserResolverService _userResolverService;


        public CertificateRepository(CRVSDbContext dbContext, IElasticClient elasticClient, IUserResolverService userResolverService) : base(dbContext)
        {
            this._dbContext = dbContext;
            _elasticClient = elasticClient;
            _userResolverService = userResolverService;
        }
        async Task<Certificate> ICertificateRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

        public async Task<IEnumerable<Certificate>> GetByEventAsync(Guid id)
        {
            return await _dbContext.Certificates.Where(c => c.EventId == id).ToListAsync();
        }
        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var entries = _dbContext.ChangeTracker
                       .Entries()
                       .Where(e => e.Entity is Certificate &&
                               (e.State == EntityState.Added
                               || e.State == EntityState.Modified));

            List<CertificateEntry> certificateEntries = entries.Select(e => new CertificateEntry
            {
                State = e.State,
                CertificateId = ((Certificate)e.Entity).Id
            }).ToList();


            var saveChangeRes = await base.SaveChangesAsync(cancellationToken);



            if (saveChangeRes)
            {
                // index add or update changes to elastic search certificate index
                if (certificateEntries.Any())
                {

                    BackgroundJob.Enqueue<IFireAndForgetJobs>(x => x.IndexCertificate(certificateEntries));
                }
            }

            return saveChangeRes;
        }
        public async Task<List<SearchCertificateResponseDTO>> SearchCertificate(SearchCertificateQuery query)
        {
            // await _elasticClient.Indices.DeleteAsync("certificate");
            // await _elasticClient.Indices.DeleteAsync("personal_info");
            // return new List<SearchCertificateResponseDTO>();
            var response = _elasticClient.SearchAsync<CertificateIndex>(s => s
                    .Index("certificate")
                    .Source(src => src
                    .Includes(i => i
                        .Fields(
                            f => f.CertificateDbId,
                            f => f.EventId,
                            f => f.NestedEventId,
                            f => f.FirstNameAm,
                            f => f.FirstNameOr,
                            f => f.MiddleNameAm,
                            f => f.MiddleNameOr,
                            f => f.LastNameAm,
                            f => f.LastNameOr,
                            f => f.AddressAm,
                            f => f.AddressOr,
                            f => f.NationalId,
                            f => f.CertificateId,
                            f => f.EventType,
                            f => f.CertificateSerialNumber,
                            f => f.CivilRegOfficerNameAm,
                            f => f.CivilRegOfficerNameOr,
                            f => f.AddressAm,
                            f => f.AddressOr,
                            f => f.EventAddressAm,
                            f => f.EventAddressOr,
                            f => f.MotherFirstNameAm,
                            f => f.MotherFirstNameOr,
                            f => f.MotherMiddleNameAm,
                            f => f.MotherMiddleNameOr,
                            f => f.MotherLastNameAm,
                            f => f.MotherLastNameOr,
                            f => f.EventRegisteredAddressId
                        )
                    ))
                    .Query(q =>
                    q
                    .Wildcard(w => w

                    .Field(f => f.CertificateSerialNumber).Value($"*{query.SearchString}*").CaseInsensitive(true)
                    ) ||
                     q
                    .Wildcard(w => w
                    .Field(f => f.ContentStr).Value($"*{query.SearchString}*").CaseInsensitive(true)
                    )
                    ).Size(50)
                    );
            return response.Result.Documents.Select(d => new SearchCertificateResponseDTO
            {
                Id = d.CertificateDbId,
                EventId = d.EventId,
                NestedEventId = d.NestedEventId,
                FullName = HelperService.getCurrentLanguage().ToLower() == "am"
                ? d.FirstNameAm + " " + d.MiddleNameAm + " " + d.LastNameAm
                : d.FirstNameOr + " " + d.MiddleNameOr + " " + d.LastNameOr,
                MotherName = HelperService.getCurrentLanguage().ToLower() == "am"
                ? d.MotherFirstNameAm + " " + d.MotherMiddleNameAm + " " + d.MotherLastNameAm
                : d.MotherFirstNameOr + " " + d.MotherMiddleNameOr + " " + d.MotherLastNameOr,
                CivilRegOfficerName = HelperService.getCurrentLanguage().ToLower() == "am"
                ? d.CivilRegOfficerNameAm
                : d.CivilRegOfficerNameOr,
                Address = HelperService.getCurrentLanguage().ToLower() == "am"
                ? d.AddressAm
                : d.AddressOr,
                EventAddress = HelperService.getCurrentLanguage().ToLower() == "am"
                ? d.EventAddressAm
                : d.EventAddressOr,
                NationalId = d.NationalId,
                CertificateId = d.CertificateId,
                EventType = d.EventType,
                CertificateSerialNumber = d.CertificateSerialNumber,
                CanViewDetail = d.EventRegisteredAddressId == _userResolverService.GetWorkingAddressId()
            }).ToList();
        }


        public async Task InitializeCertificateIndex()
        {
            if (!_elasticClient.Indices.Exists("certificate").Exists)
            {
                _elasticClient
                                 .IndexMany<CertificateIndex>(_dbContext.Certificates
                                     .Select(c => new CertificateIndex
                                     {
                                         CertificateDbId = c.Id,
                                         EventId = c.Event.Id,
                                         EventType = c.Event.EventType,
                                         NestedEventId = c.Event.EventType.ToLower() == "birth"
                                                          ? c.Event.BirthEvent.Id
                                                          : c.Event.EventType.ToLower() == "death"
                                                          ? c.Event.DeathEventNavigation.Id
                                                          : c.Event.EventType.ToLower() == "marriage"
                                                          ? c.Event.MarriageEvent.Id
                                                          : c.Event.EventType.ToLower() == "adoption"
                                                          ? c.Event.AdoptionEvent.Id
                                                          : c.Event.EventType == "divorce"
                                                          ? c.Event.DivorceEvent.Id : null,
                                         MotherFirstNameAm = c.Event.EventType.ToLower() == "birth"
                                                           ? (c.Event.BirthEvent.Mother.FirstName == null ? null : c.Event.BirthEvent.Mother.FirstName.Value<string>("am"))
                                                           : c.Event.EventType.ToLower() == "adoption" ?
                                                           (c.Event.AdoptionEvent.AdoptiveMother.FirstName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.FirstName.Value<string>("am"))
                                                           : null,
                                         MotherFirstNameOr = c.Event.EventType.ToLower() == "birth"
                                                           ? (c.Event.BirthEvent.Mother.FirstName == null ? null : c.Event.BirthEvent.Mother.FirstName.Value<string>("or"))
                                                           : c.Event.EventType.ToLower() == "adoption" ?
                                                           (c.Event.AdoptionEvent.AdoptiveMother.FirstName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.FirstName.Value<string>("or"))
                                                           : null,
                                         MotherMiddleNameAm = c.Event.EventType.ToLower() == "birth"
                                                           ? (c.Event.BirthEvent.Mother.MiddleName == null ? null : c.Event.BirthEvent.Mother.MiddleName.Value<string>("am"))
                                                           : c.Event.EventType.ToLower() == "adoption" ?
                                                           (c.Event.AdoptionEvent.AdoptiveMother.MiddleName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.MiddleName.Value<string>("am"))
                                                           : null,
                                         MotherMiddleNameOr = c.Event.EventType.ToLower() == "birth"
                                                           ? (c.Event.BirthEvent.Mother.MiddleName == null ? null : c.Event.BirthEvent.Mother.MiddleName.Value<string>("or"))
                                                           : c.Event.EventType.ToLower() == "adoption" ?
                                                           (c.Event.AdoptionEvent.AdoptiveMother.MiddleName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.MiddleName.Value<string>("or"))
                                                           : null,
                                         MotherLastNameAm = c.Event.EventType.ToLower() == "birth"
                                                           ? (c.Event.BirthEvent.Mother.LastName == null ? null : c.Event.BirthEvent.Mother.LastName.Value<string>("am"))
                                                           : c.Event.EventType.ToLower() == "adoption" ?
                                                           (c.Event.AdoptionEvent.AdoptiveMother.LastName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.LastName.Value<string>("am"))
                                                           : null,
                                         MotherLastNameOr = c.Event.EventType.ToLower() == "birth"
                                                           ? (c.Event.BirthEvent.Mother.LastName == null ? null : c.Event.BirthEvent.Mother.LastName.Value<string>("or"))
                                                           : c.Event.EventType.ToLower() == "adoption" ?
                                                           (c.Event.AdoptionEvent.AdoptiveMother.LastName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.LastName.Value<string>("or"))
                                                           : null,
                                         CivilRegOfficerNameAm = c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.FirstName == null ? " " : c.Event.CivilRegOfficer.FirstName.Value<string>("am")
                                                               + c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.MiddleName == null ? " " : c.Event.CivilRegOfficer.MiddleName.Value<string>("am")
                                                               + c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.LastName == null ? " " : c.Event.CivilRegOfficer.LastName.Value<string>("am"),
                                         CivilRegOfficerNameOr = c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.FirstName == null ? " " : c.Event.CivilRegOfficer.FirstName.Value<string>("or")
                                                               + c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.MiddleName == null ? " " : c.Event.CivilRegOfficer.MiddleName.Value<string>("or")
                                                               + c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.LastName == null ? " " : c.Event.CivilRegOfficer.LastName.Value<string>("or"),
                                         CertificateId = c.Event.CertificateId,
                                         CertificateSerialNumber = c.CertificateSerialNumber,
                                         ContentStr = c.ContentStr,
                                         AddressAm = c.Event.EventOwener.ResidentAddress == null ? null : c.Event.EventOwener.ResidentAddress.AddressName.Value<string>("am"),
                                         AddressOr = c.Event.EventOwener.ResidentAddress == null ? null : c.Event.EventOwener.ResidentAddress.AddressName.Value<string>("or"),
                                         NationalId = c.Event.EventOwener.NationalId,
                                         FirstNameOr = c.Event.EventOwener.FirstName == null ? null : c.Event.EventOwener.FirstName.Value<string>("or"),
                                         FirstNameAm = c.Event.EventOwener.FirstName == null ? null : c.Event.EventOwener.FirstName.Value<string>("am"),
                                         MiddleNameOr = c.Event.EventOwener.MiddleName == null ? null : c.Event.EventOwener.MiddleName.Value<string>("or"),
                                         MiddleNameAm = c.Event.EventOwener.MiddleName == null ? null : c.Event.EventOwener.MiddleName.Value<string>("am"),
                                         LastNameOr = c.Event.EventOwener.LastName == null ? null : c.Event.EventOwener.LastName.Value<string>("or"),
                                         LastNameAm = c.Event.EventOwener.LastName == null ? null : c.Event.EventOwener.LastName.Value<string>("am"),
                                         EventAddressAm = c.Event.EventAddress == null ? null : c.Event.EventAddress.AddressName.Value<string>("am"),
                                         EventAddressOr = c.Event.EventAddress == null ? null : c.Event.EventAddress.AddressName.Value<string>("or"),
                                         EventRegisteredAddressId = c.Event.EventRegisteredAddressId

                                     }), "certificate");
            }
        }

        public async Task<(BirthEvent? birth, DeathEvent? death, AdoptionEvent? adoption, MarriageEvent? marriage, DivorceEvent? divorce)> GetContent(Guid eventId)
        {
            var eventType = _dbContext.Events.Where(e => e.Id == eventId).Select(e => e.EventType).FirstOrDefault();
            switch (eventType)
            {
                case "Birth":
                    return (await _dbContext.BirthEvents
                            .Include(d => d.Father).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Mother).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.SexLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.BirthAddress)
                            .Include(d => d.Event).ThenInclude(e => e.CivilRegOfficer)
                            .Include(d => d.Event).ThenInclude(e => e.EventAddress)
                            .Where(e => e.EventId == eventId).FirstOrDefaultAsync()
                            , null, null, null, null);
                case "Death":
                    return (null,
                            await _dbContext.DeathEvents
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.TitleLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.SexLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.TitleLookup)
                            .Include(d => d.Event).ThenInclude(e => e.CivilRegOfficer)
                            .Include(d => d.Event).ThenInclude(e => e.EventAddress)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener)
                            .Where(e => e.EventId == eventId).FirstOrDefaultAsync()
                            , null, null, null);
                case "Adoption":
                    return (null, null,
                            await _dbContext.AdoptionEvents
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.SexLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.BirthAddress)
                            .Include(d => d.AdoptiveFather).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.AdoptiveMother).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Event).ThenInclude(e => e.CivilRegOfficer)
                            .Where(e => e.EventId == eventId).FirstOrDefaultAsync()
                            , null, null);
                case "Marriage":
                    return (null, null, null,
                            await _dbContext.MarriageEvents
                                .Include(m => m.BrideInfo).ThenInclude(p => p.NationalityLookup)
                                .Include(m => m.Event).ThenInclude(e => e.EventAddress)
                                .Include(m => m.Event.EventOwener).ThenInclude(p => p.NationalityLookup)
                                .Include(d => d.Event).ThenInclude(e => e.CivilRegOfficer)
                                .Where(m => m.EventId == eventId).FirstOrDefaultAsync(), null);
                case "Divorce":
                    return (null, null, null, null,
                            await _dbContext.DivorceEvents
                                .Include(m => m.CourtCase)
                                .Include(m => m.DivorcedWife).ThenInclude(p => p.NationalityLookup)
                                .Include(m => m.DivorcedWife).ThenInclude(p => p.BirthAddress)
                                .Include(m => m.Event).ThenInclude(e => e.EventAddress)
                                .Include(m => m.Event.EventOwener).ThenInclude(p => p.NationalityLookup)
                                .Include(m => m.Event.EventOwener).ThenInclude(p => p.BirthAddress)
                                .Include(d => d.Event).ThenInclude(e => e.CivilRegOfficer)
                                .Where(m => m.EventId == eventId).FirstOrDefaultAsync());
            }
            return (null, null, null, null, null);
        }


    }

}