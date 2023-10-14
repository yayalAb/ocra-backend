using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class SupportingDocumentRepository : BaseRepository<SupportingDocument>, ISupportingDocumentRepository
    {
        private readonly CRVSDbContext _dbContext;

        public SupportingDocumentRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public (string BrideImage, string GroomImage) MarriageImage(Guid eventId)
        {
            var events = _dbContext.Events
                    .Include(e => e.MarriageEvent)
                    .SingleOrDefault(e => e.Id == eventId);
            return (events.MarriageEvent.BrideInfoId.ToString(), events.EventOwenerId.ToString());

            // string brideImage = base.GetAll()
            //     .OrderByDescending(s => s.CreatedAt)
            //     .Where(s => s.Label == "Bride" && (s.TypeLookup.ValueStr.ToLower().Contains("webcam")))
            //     .Select(s => s.Id)
            //     .FirstOrDefault()
            //     .ToString();
            // string groomImage = base.GetAll()
            //     .OrderByDescending(s => s.CreatedAt)
            //     .Where(s => s.Label == "Groom" && (s.TypeLookup.ValueStr.ToLower().Contains("webcam")))
            //     .Select(s => s.Id)
            //     .FirstOrDefault()
            //     .ToString();

            // return (brideImage, groomImage);
        }
    }
}