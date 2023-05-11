using AppDiv.CRVS.Application.Contracts.Request;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class CourtListDTO
    {
        public Guid id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}