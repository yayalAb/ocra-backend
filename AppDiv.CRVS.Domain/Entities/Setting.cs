
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Setting : BaseAuditableEntity
    {
        public string Key { get; set; }
        public Json value { get; set; }
    }
}