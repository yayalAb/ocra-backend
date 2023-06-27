using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class SupportingDocument : BaseAuditableEntity
    {
        public Guid? EventId { get; set; }
        public Guid? PaymentExamptionId { get; set;}
        public string? Description { get; set; }
        public string DocumentUrl { get; set; }
        public string Type {get; set;}
        public string Label { get; set; }

        public virtual Event Event { get; set; }
        public virtual PaymentExamption PaymentExamption { get; set;}

        [NotMapped] 
        public string base64String { get; set; }

        public SupportingDocument()
        {
            this.DocumentUrl = "Url";
        }
    }
}