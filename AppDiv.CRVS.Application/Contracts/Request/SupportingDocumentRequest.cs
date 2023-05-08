using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class SupportingDocumentRequest
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string DocumentUrl { get; set; }
        [NotMapped]
        public string FileStr { get; set; }

        public SupportingDocumentRequest()
        {
            this.Id = new Guid();
            this.DocumentUrl = "demo";
        }
    }
}