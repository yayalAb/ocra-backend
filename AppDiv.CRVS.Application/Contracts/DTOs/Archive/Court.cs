using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class CourtArchive
    {
        public string? CourtNameOr { get; set; }
        public string? CourtNameAm { get; set; }

        public string? CourtAddressOr { get; set; }
        public string? CourtAddressAm { get; set; }

        public string? CourtConfirmationMonthOr { get; set; }
        public string? CourtConfirmationMonthAm { get; set; }
        public string? CourtConfirmationDay { get; set; }
        public string? CourtConfirmationYear { get; set; }

        public string CourtCaseNumber { get; set; }
    }
}