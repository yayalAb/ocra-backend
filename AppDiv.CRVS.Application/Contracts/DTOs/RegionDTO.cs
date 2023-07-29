namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class RegionDTO
    {

        public Guid Id { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string StatisticCode { get; set; }
        public string Code { get; set; }
        public string? AdminType { get; set; }
        public Guid? AreaTypeLookupId { get; set; }
        public Guid? ParentAddressId { get; set; }

    }
}