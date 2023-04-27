namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class KebeleDTO
    {
        public Guid Id { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }

        public string? Zone { get; set; }
        public string? Woreda { get; set; }
        public string? Kebele { get; set; }
        public string StatisticCode { get; set; }
        public string Code { get; set; }
    }
}