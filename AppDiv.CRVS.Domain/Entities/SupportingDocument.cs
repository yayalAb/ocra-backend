using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Domain.Models;

namespace AppDiv.CRVS.Domain.Entities
{
    public class SupportingDocument : BaseAuditableEntity
    {
        public Guid? EventId { get; set; }
        public Guid? PaymentExamptionId { get; set; }
        public string? Description { get; set; }
        public string DocumentUrl { get; set; }
        public Guid Type { get; set; }//typeLookupId
        public string Label { get; set; }

        public virtual Event Event { get; set; }
        public virtual PaymentExamption PaymentExamption { get; set; }
        public virtual Lookup TypeLookup { get; set; }

        [NotMapped]
        public string base64String { get; set; } = string.Empty;

        [NotMapped]
        public List<BiometricImagesAtt>? FingerPrint { get; set; }

        public SupportingDocument()
        {
            this.DocumentUrl = "Url";
        }
    }
}