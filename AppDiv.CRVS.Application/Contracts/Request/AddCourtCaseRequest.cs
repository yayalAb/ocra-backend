namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddCourtCaseRequest
    {
        public Guid CourtId { get; set; }
        public string CourtCaseNumber { get; set; }
        public DateTime ConfirmedDate { get; set; }
    }
}