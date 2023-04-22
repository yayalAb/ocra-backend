namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddLookupRequest
    {
        public string Key { get; set; }
        public string valueStr { get; set; }
        public string descriptionStr { get; set; }
        public string StatisticCode { get; set; }
        public string Code { get; set; }
    }
}