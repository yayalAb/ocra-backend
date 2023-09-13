using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Exceptions;
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
            Guid workingAddressId = _userResolverService.GetWorkingAddressId();
            if (workingAddressId == Guid.Empty)
            {
                throw new NotFoundException("Invalid user working address please login first");
            }
            string? workingAddressIdStr = workingAddressId.ToString();
            var response = await _elasticClient.SearchAsync<CertificateIndex>(s => s
                    .Index("certificate")
                    .Source(src => src
                    .Includes(i => i
                        .Fields(
                            f => f.Id,
                            f => f.EventId,
                            f => f.NestedEventId,
                            f => f.FullNameAm,
                            f => f.FullNameOr,
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
                            f => f.MotherFullNameAm,
                            f => f.MotherFullNameOr,
                            f => f.EventRegisteredAddressId,
                            f => f.EventRegisteredAddressAm,
                            f => f.EventRegisteredAddressOr,
                            f => f.Status
                        )
                    ))
                    .Query(q =>
                    q.Bool(b => b
                    .Must(
                            mu => mu.Match(m => m.Field(f => f.Status).Query("true")
                            )
                            ,
                            
                            mu => mu.QueryString(d => d.Query('*' + workingAddressIdStr + '*'))

                        // mu =>mu.Term(t => t.Field(f => f.EventRegisteredAddressId).Value(workingAddressIdStr))


                        ))
                        &&
                    (q.Wildcard(w => w
                            .Field(f => f.CertificateSerialNumber).Value($"*{query.SearchString}*").CaseInsensitive(true)
                            ) ||
                            q
                            .Wildcard(w => w
                            .Field(f => f.ContentStr).Value($"*{query.SearchString}*").CaseInsensitive(true)
                            ) ||
                            q
                            .Wildcard(w => w
                            .Field(f => f.FullNameAm).Value($"*{query.SearchString}*").CaseInsensitive(true)
                            ) ||
                            q
                            .Wildcard(w => w
                            .Field(f => f.FullNameOr).Value($"*{query.SearchString}*").CaseInsensitive(true)
                            ) ||
                            q
                            .Wildcard(w => w
                            .Field(f => f.MotherFullNameAm).Value($"*{query.SearchString}*").CaseInsensitive(true)
                            ) ||
                            q
                            .Wildcard(w => w
                            .Field(f => f.MotherFullNameOr).Value($"*{query.SearchString}*").CaseInsensitive(true)
                            ))
                    ).Size(50)
                    );
            string currentLanguage  = HelperService.getCurrentLanguage().ToLower();

            bool isAmharic = currentLanguage == "am" ;

            return response.Documents.Select(d => new SearchCertificateResponseDTO
            {
                Id = d.Id,
                EventId = d.EventId,
                NestedEventId = d.NestedEventId,
                FullName = isAmharic ? d.FullNameAm : d.FullNameOr,
                MotherName = isAmharic ? d.MotherFullNameAm : d.MotherFullNameOr,
                CivilRegOfficerName = isAmharic ? d.CivilRegOfficerNameAm : d.CivilRegOfficerNameOr,
                Address = isAmharic ? d.AddressAm  : d.AddressOr,
                EventAddress = isAmharic ? d.EventAddressAm : d.EventAddressOr,
                EventRegisteredAddress = isAmharic ? d.EventRegisteredAddressAm : d.EventRegisteredAddressOr,
                NationalId = d.NationalId,
                CertificateId = d.CertificateId,
                EventType = d.EventType,
                CertificateSerialNumber = d.CertificateSerialNumber,
                CanViewDetail = d.EventRegisteredAddressId == workingAddressIdStr,
                Status = d.Status
            }).ToList();
        }


        public async Task InitializeCertificateIndex(bool reIndex = false)
        {
            if (reIndex)
            {
                await _elasticClient.Indices.DeleteAsync("certificate");

            }
            if (!_elasticClient.Indices.Exists("certificate").Exists && _dbContext.Certificates.Any())
            {
                _elasticClient
                                 .IndexMany<CertificateIndex>(_dbContext.Certificates
                                 .Where(c => c.Status)
                                     .Select(c => new CertificateIndex
                                     {
                                         Id = c.Id,
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
                                         MotherFullNameAm = c.Event.EventType.ToLower() == "birth"
                                                           ?
                                                            (c.Event.BirthEvent.Mother.FirstName == null ? null : c.Event.BirthEvent.Mother.FirstName.Value<string>("am"))
                                                            + " " + (c.Event.BirthEvent.Mother.MiddleName == null ? null : c.Event.BirthEvent.Mother.MiddleName.Value<string>("am"))
                                                            + " " + (c.Event.BirthEvent.Mother.LastName == null ? null : c.Event.BirthEvent.Mother.LastName.Value<string>("am"))
                                                           : c.Event.EventType.ToLower() == "adoption"
                                                           ?
                                                            (c.Event.AdoptionEvent.AdoptiveMother.FirstName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.FirstName.Value<string>("am"))
                                                            + " " + (c.Event.AdoptionEvent.AdoptiveMother.MiddleName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.MiddleName.Value<string>("am"))
                                                            + " " + (c.Event.AdoptionEvent.AdoptiveMother.LastName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.LastName.Value<string>("am"))
                                                           : null,
                                         MotherFullNameOr = c.Event.EventType.ToLower() == "birth"
                                                           ?
                                                            (c.Event.BirthEvent.Mother.FirstName == null ? null : c.Event.BirthEvent.Mother.FirstName.Value<string>("or"))
                                                            + " " + (c.Event.BirthEvent.Mother.MiddleName == null ? null : c.Event.BirthEvent.Mother.MiddleName.Value<string>("or"))
                                                            + " " + (c.Event.BirthEvent.Mother.LastName == null ? null : c.Event.BirthEvent.Mother.LastName.Value<string>("or"))
                                                           : c.Event.EventType.ToLower() == "adoption"
                                                           ?
                                                            (c.Event.AdoptionEvent.AdoptiveMother.FirstName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.FirstName.Value<string>("or"))
                                                            + " " + (c.Event.AdoptionEvent.AdoptiveMother.MiddleName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.MiddleName.Value<string>("or"))
                                                            + " " + (c.Event.AdoptionEvent.AdoptiveMother.LastName == null ? null : c.Event.AdoptionEvent.AdoptiveMother.LastName.Value<string>("or"))
                                                           : null,
                                         CivilRegOfficerNameAm = (c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.FirstName == null ? " " : c.Event.CivilRegOfficer.FirstName.Value<string>("am"))
                                                             + " " + (c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.MiddleName == null ? " " : c.Event.CivilRegOfficer.MiddleName.Value<string>("am"))
                                                             + " " + (c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.LastName == null ? " " : c.Event.CivilRegOfficer.LastName.Value<string>("am")),
                                         CivilRegOfficerNameOr = (c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.FirstName == null ? " " : c.Event.CivilRegOfficer.FirstName.Value<string>("or"))
                                                             + " " + (c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.MiddleName == null ? " " : c.Event.CivilRegOfficer.MiddleName.Value<string>("or"))
                                                             + " " + (c.Event.CivilRegOfficer == null || c.Event.CivilRegOfficer.LastName == null ? " " : c.Event.CivilRegOfficer.LastName.Value<string>("or")),
                                         CertificateId = c.Event.CertificateId,
                                         CertificateSerialNumber = c.CertificateSerialNumber,
                                         ContentStr = c.ContentStr,
                                         AddressAm = c.Event.EventOwener.ResidentAddress == null ? null : c.Event.EventOwener.ResidentAddress.AddressName.Value<string>("am"),
                                         AddressOr = c.Event.EventOwener.ResidentAddress == null ? null : c.Event.EventOwener.ResidentAddress.AddressName.Value<string>("or"),
                                         NationalId = c.Event.EventOwener.NationalId,
                                         FullNameAm = (c.Event.EventOwener.FirstName == null ? null : c.Event.EventOwener.FirstName.Value<string>("am"))
                                                        + " " + (c.Event.EventOwener.MiddleName == null ? null : c.Event.EventOwener.MiddleName.Value<string>("am"))
                                                        + " " + (c.Event.EventOwener.LastName == null ? null : c.Event.EventOwener.LastName.Value<string>("am")),
                                         FullNameOr = (c.Event.EventOwener.FirstName == null ? null : c.Event.EventOwener.FirstName.Value<string>("or"))
                                                        + " " + (c.Event.EventOwener.MiddleName == null ? null : c.Event.EventOwener.MiddleName.Value<string>("or"))
                                                        + " " + (c.Event.EventOwener.LastName == null ? null : c.Event.EventOwener.LastName.Value<string>("or")),
                                         EventAddressAm = c.Event.EventAddress == null ? null : c.Event.EventAddress.AddressName.Value<string>("am"),
                                         EventAddressOr = c.Event.EventAddress == null ? null : c.Event.EventAddress.AddressName.Value<string>("or"),
                                         EventRegisteredAddressId = c.Event.EventRegisteredAddressId == null ? null : c.Event.EventRegisteredAddressId.ToString(),
                                         EventRegisteredAddressAm = c.Event.EventRegisteredAddress == null ? null : c.Event.EventRegisteredAddress.AddressName.Value<string>("am"),
                                         EventRegisteredAddressOr = c.Event.EventRegisteredAddress == null ? null : c.Event.EventRegisteredAddress.AddressName.Value<string>("or"),
                                         Status = c.Status
                                     }), "certificate");
                await _elasticClient.Indices.RefreshAsync("certificate");

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
                            .Include(d => d.Event)
                            .ThenInclude(e => e.EventOwener)
                            .Include(d => d.Event)
                            .ThenInclude(e => e.EventOwener)
                            .ThenInclude(p => p.SexLookup)
                            .Include(d => d.Event)
                            .ThenInclude(e => e.EventOwener)
                            .ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Event)
                            .ThenInclude(e => e.EventOwener).ThenInclude(p => p.BirthAddress)
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

        public async Task MakeCertificateVoid(Guid eventId, CancellationToken cancellationToken)
        {
            var certificate = _dbContext.Certificates
                                .Where(c => c.EventId == eventId && c.Status).FirstOrDefault();
            if (certificate != null)
            {
                certificate.Status = false;
                base.Update(certificate);
                await this.SaveChangesAsync(cancellationToken);
            }
        }
    }

}