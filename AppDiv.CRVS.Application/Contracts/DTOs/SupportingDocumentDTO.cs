using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class SupportingDocumentDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
    }
}