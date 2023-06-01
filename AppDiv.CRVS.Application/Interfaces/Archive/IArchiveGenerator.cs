using AppDiv.CRVS.Application.Features.Archives.Query;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IArchiveGenerator
    {
        public JObject GetArchive(GenerateArchiveQuery request, Event? content, string BirhtCertId);

    }
}