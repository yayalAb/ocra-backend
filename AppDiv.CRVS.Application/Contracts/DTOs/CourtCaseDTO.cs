using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class CourtCaseDTO
    {
        public Guid? Id { get; set; }
        public CourtDTO Court { get; set; }
        public string CourtCaseNumber { get; set; }
        public DateTime ConfirmedDate { get; set; }

    }
}