using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddCourtRequest
    {
        public virtual Guid? AddressId { get; set; }
        public JObject Name { get; set; }
        public JObject? Description { get; set; }
    }
}