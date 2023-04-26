namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ZoneDTO
    {
        public Guid id { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }

        public string? Zone { get; set; }
    }
}