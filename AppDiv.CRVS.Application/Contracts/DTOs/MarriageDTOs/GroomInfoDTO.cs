
namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class GroomInfoDTO : UpdatePersonalInfoDTO
    {
        public Guid BirthAddressId { get; set; }
        public DateTime BirthDate { get; set; }
// 

    }
}