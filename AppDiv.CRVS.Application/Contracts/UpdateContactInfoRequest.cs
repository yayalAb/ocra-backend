namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class UpdateContactInfoRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string? Website { get; set; }
        public string? Phone { get; set; }
        public string? HouseNo { get; set; }
        public string? Linkdin { get; set; }
    }
}