namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class MarriageApplicationSearchDto
    {
        public Guid Id { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string GroomFullName { get; set; }
        public string BridFullName { get; set; }
        public string CicilRegOfficerFullName { get; set; }
    }
}

