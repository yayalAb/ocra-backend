

namespace AppDiv.CRVS.Application.Contracts.DTOs;
public class SearchCertificateResponseDTO
{
    public Guid Id { get; set; }
    public Guid? EventId { get;set;} 
    public Guid? NestedEventId { get;set;} 
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public string? CertificateId { get; set; }
    public string? EventType { get; set; }
    public string? CertificateSerialNumber { get; set; }
}