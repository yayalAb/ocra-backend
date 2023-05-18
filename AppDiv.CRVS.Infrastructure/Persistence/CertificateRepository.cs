using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CertificateRepository : BaseRepository<Certificate>, ICertificateRepository
    {
        private readonly CRVSDbContext _dbContext;
        public CertificateRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }
        async Task<Certificate> ICertificateRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

        public async Task<IEnumerable<Certificate>> GetByEventAsync(Guid id)
        {
            return await _dbContext.Certificates.Where(c => c.EventId == id).ToListAsync();
        }

        public async Task<(BirthEvent? birth, DeathEvent? death, AdoptionEvent? adoption, MarriageEvent? marriage, DivorceEvent? divorce)> GetContent(Guid eventId)
        {
            var eventType = _dbContext.Events.Where(e => e.Id == eventId).Select(e => e.EventType).FirstOrDefault();
            switch (eventType)
            {
                case "BirthEvent":
                    return (await _dbContext.BirthEvents
                            .Include(d => d.Father).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Mother).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.SexLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Event).ThenInclude(e => e.CivilRegOfficer)
                            .Include(d => d.Event).ThenInclude(e => e.EventAddress)
                            .Where(e => e.EventId == eventId).FirstOrDefaultAsync()
                            , null, null, null, null);
                case "DeathEvent":
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
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener)
                            .Include(d => d.Event).ThenInclude(e => e.EventAddress)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.NationalityLookup)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.BirthAddress)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener).ThenInclude(p => p.SexLookup)
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
                                .Include(m => m.Event).ThenInclude(e => e.EventAddress)
                                .Include(m => m.Event.EventOwener).ThenInclude(p => p.NationalityLookup)
                                .Include(d => d.Event).ThenInclude(e => e.CivilRegOfficer)
                                .Where(m => m.EventId == eventId).FirstOrDefaultAsync());
            }
            return (null, null, null, null, null);
        }
    }
}