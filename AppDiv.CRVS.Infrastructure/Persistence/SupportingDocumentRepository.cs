using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class SupportingDocumentRepository : BaseRepository<SupportingDocument>, ISupportingDocumentRepository
    {
        public SupportingDocumentRepository(CRVSDbContext dbContext) : base(dbContext)
        {
        }

        public (string BrideImage, string GroomImage) MarriageImage()
        {
            string brideImage = base.GetAll().Where(s => s.Label == "Bride" && (s.Type == "WebCam" || s.Type == "FaceId")).Select(s => s.Id).FirstOrDefault().ToString();
            string groomImage = base.GetAll().Where(s => s.Label == "Groom" && (s.Type == "WebCam" || s.Type == "FaceId")).Select(s => s.Id).FirstOrDefault().ToString();

            return (brideImage, groomImage);
        }
    }
}