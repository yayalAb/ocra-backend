namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class SearchEventResponseDTO
    {
        public Guid Id { get; set; }
        public string? CertifceteType { get; set; }
        public string? CertifceteId { get; set; }
        public string? booknumber { get; set; }
        public string? FullName { get; set; }

        public string? CivilRegOfficer { get; set; }

    }
}


