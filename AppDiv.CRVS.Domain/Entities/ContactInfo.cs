using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Domain.Entities.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Domain.Entities
{
    public class ContactInfo : BaseAuditableEntity
    {
      public string Email { get; set; }
      public string? Phone { get; set; }
      public string? HouseNumber { get; set; }
      public string? Website { get; set; }
      public string? Linkdin { get; set;}
      public Guid PersonalInfoId  { get; set;}

      public virtual PersonalInfo PersonalInfo { get; set; }
        
        
    }
}
