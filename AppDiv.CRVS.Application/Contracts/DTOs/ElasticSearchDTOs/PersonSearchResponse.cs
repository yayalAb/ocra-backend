
namespace AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs
{
    public class PersonSearchResponse
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Address { get;set; }
        public string? NationalId { get; set; }
    }
}