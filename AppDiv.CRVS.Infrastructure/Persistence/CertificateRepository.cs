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
using AppDiv.CRVS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Nest;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CertificateRepository : BaseRepository<Certificate>, ICertificateRepository
    {
        private readonly CRVSDbContext _dbContext;
        private readonly IElasticClient _elasticClient;

        public CertificateRepository(CRVSDbContext dbContext, IElasticClient elasticClient) : base(dbContext)
        {
            this._dbContext = dbContext;
            _elasticClient = elasticClient;
        }
        async Task<Certificate> ICertificateRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

        public async Task<IEnumerable<Certificate>> GetByEventAsync(Guid id)
        {
            return await _dbContext.Certificates.Where(c => c.EventId == id).ToListAsync();
        }
 public async Task<List<SearchCertificateResponseDTO>> SearchCertificate(SearchCertificateQuery query)
        {
            // _elasticClient.Indices.Delete("certificate");

            if (!_elasticClient.Indices.Exists("certificate").Exists)
            {

                var indexRes = _elasticClient
                   .IndexMany<CertificateIndex>(_dbContext.Certificates
                       .Select(c => new CertificateIndex
                       {
                           Id = c.Id,
                           EventId = c.Event.Id,
                           EventType = c.Event.EventType,
                           CertificateId = c.Event.CertificateId,
                           CertificateSerialNumber = c.CertificateSerialNumber,
                           ContentStr = c.ContentStr,
                           AddressAm = c.Event.EventOwener.ResidentAddress == null?null :c.Event.EventOwener.ResidentAddress.AddressName.Value<string>("am"),
                           AddressOr = c.Event.EventOwener.ResidentAddress == null?null :c.Event.EventOwener.ResidentAddress.AddressName.Value<string>("or"),
                           NationalId = c.Event.EventOwener.NationalId,
                           FirstNameOr = c.Event.EventOwener.FirstName == null ? null : c.Event.EventOwener.FirstName.Value<string>("or"),
                           FirstNameAm = c.Event.EventOwener.FirstName == null ? null : c.Event.EventOwener.FirstName.Value<string>("am"),
                           MiddleNameOr = c.Event.EventOwener.MiddleName == null ? null : c.Event.EventOwener.MiddleName.Value<string>("or"),
                           MiddleNameAm = c.Event.EventOwener.MiddleName == null ? null : c.Event.EventOwener.MiddleName.Value<string>("am"),
                           LastNameOr = c.Event.EventOwener.LastName == null ? null : c.Event.EventOwener.LastName.Value<string>("or"),
                           LastNameAm = c.Event.EventOwener.LastName == null ? null : c.Event.EventOwener.LastName.Value<string>("am"),

                       }).ToList(), "certificate");
            }

            var response = _elasticClient.SearchAsync<CertificateIndex>(s => s
                    .Index("certificate")
                    .Source(src => src
                    .Includes(i => i
                        .Fields(
                            f => f.Id,
                            f => f.EventId,
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
                            f => f.CertificateSerialNumber
                            

                        )
                    ))
                    .Query(q =>
                    q
                    .Wildcard(w => w
                    .Field(f => f.CertificateSerialNumber).Value($"*{query.SearchString}*")
                    ) ||
                     q
                    .Wildcard(w => w
                    .Field(f => f.ContentStr).Value($"*{query.SearchString}*")
                    )
                    ).Size(50)
                    );
            return response.Result.Documents.Select(d => new SearchCertificateResponseDTO
            {
                Id = d.Id,
                EventId = d.EventId,
                FullName = HelperService.getCurrentLanguage().ToLower() == "am"
                ? d.FirstNameAm + " " + d.MiddleNameAm + " " + d.LastNameAm
                : d.FirstNameOr + " " + d.MiddleNameOr + " " + d.LastNameOr,
                Address = HelperService.getCurrentLanguage().ToLower() == "am"
                ? d.AddressAm
                : d.AddressOr,
                NationalId = d.NationalId,
                CertificateId = d.CertificateId,
                EventType = d.EventType,
                CertificateSerialNumber = d.CertificateSerialNumber
            }).ToList();
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