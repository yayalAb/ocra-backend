using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AuditLogDTO
    {
        public Guid AuditId { get; set; }
        public string Action { get; set; }
        public string Enviroment { get; set; }
        public string EntityType { get; set; }
        public DateTime AuditDate { get; set; }
        public Guid? AuditUserId { get; set; }
        public string TablePk { get; set; }
        public JObject AuditDataJson { get; set; }
        public string IpAddress { get; set; }



    }
}