using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddCourtCaseRequest
    {
        public Guid? Id { get; set; }
        public AddCourtRequest? Court { get; set; }
        public string? CourtCaseNumber { get; set; }
        // public DateTime ConfirmedDate { get; set; }
        public string? ConfirmedDateEt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }


    }
}