using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AllCountryDto
    {
        public Guid Id { get; set; }
         public string? nameOr { get; set; }
         public string? nameAm { get; set; }
         public int? adminLevel { get; set; }
         public string? adminTypeAm { get; set; }
         public string? adminTypeOr { get; set; }
         public bool? mergeStatus { get; set; }
    }
}