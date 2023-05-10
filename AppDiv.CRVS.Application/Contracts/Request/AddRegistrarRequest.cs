using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddRegistrarRequest
    {
        public Guid RelationshipId { get; set; }
        public virtual UpdatePersonalInfoRequest RegistrarInfo { get; set; }

    }
}