using AppDiv.CRVS.Application.Features.Archives.Query;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IArchiveGenerator
    {
        public JObject GetArchive(GenerateArchiveQuery request, Event? content, string BirhtCertId);
        public JObject GetBirthArchivePreview(BirthEvent birth, string BirthCertNo);
        public JObject GetAdoptionArchivePreview(AdoptionEvent adoption, string? BirthCertNo);
        public JObject GetMarriageArchivePreview(MarriageEvent marriage, string? BirthCertNo);
        public JObject GetDivorceArchivePreview(DivorceEvent divorce, string? BirthCertNo);
        public JObject GetDeathArchivePreview(DeathEvent death, string? BirthCertNo);

    }
}