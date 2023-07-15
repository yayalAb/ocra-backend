﻿using Audit.EntityFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDiv.CRVS.Domain.Entities.Audit
{
    [AuditIgnore]
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AuditId { get; set; }
        public string AuditData { get; set; }
        public string Action { get; set; }
        public string Enviroment { get; set; }
        public string EntityType { get; set; }
        public DateTime AuditDate { get; set; }
        public Guid? AuditUserId { get; set; }
        public string TablePk { get; set; }
        [NotMapped]
        public JObject AuditDataJson
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(AuditData) ? "{}" : AuditData);
            }
        }

        [NotMapped]
        public JObject EnviromentJson
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(Enviroment) ? "{}" : Enviroment);
            }
        }

    }
}
