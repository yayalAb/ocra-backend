using AppDiv.CRVS.Application.Features.Archives.Query;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IArchiveGenerator
    {
        public JObject GetArchive(GenerateArchiveQuery request, Event? content, string BirhtCertId, bool isCorrectionRequest=false);
        public JObject GetBirthArchivePreview(BirthEvent birth, string BirthCertNo,bool isCorrectionRequest=false);
        public JObject GetAdoptionArchivePreview(AdoptionEvent adoption, string? BirthCertNo,bool isCorrectionRequest=false);
        public JObject GetMarriageArchivePreview(MarriageEvent marriage, string? BirthCertNo,bool isCorrectionRequest=false);
        public JObject GetDivorceArchivePreview(DivorceEvent divorce, string? BirthCertNo,bool isCorrectionRequest=false);
        public JObject GetDeathArchivePreview(DeathEvent death, string? BirthCertNo,bool isCorrectionRequest=false);

    }
}