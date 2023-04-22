namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ListOfLookupDTO
    {
        public string Key { get; set; }
        public IEnumerable<LookupDTO> Value { get; set; }
    }
}