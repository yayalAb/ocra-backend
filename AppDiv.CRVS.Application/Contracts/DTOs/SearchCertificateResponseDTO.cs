

using Microsoft.AspNetCore.Diagnostics;

namespace AppDiv.CRVS.Application.Contracts.DTOs;
public class SearchCertificateResponseDTO
{
    public Guid Id { get; set; }
    public Guid? EventId { get;set;} 
    public Guid? NestedEventId { get;set;} 
    public string? FullName { get; set; }
    public string? MotherName {get; set ;}
    public string? CivilRegOfficerName {get; set;}
    public string? EventAddress {get; set; }
    public string? EventRegisteredAddress {get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public string? CertificateId { get; set; }
    public string? EventType { get; set; }
    public string? CertificateSerialNumber { get; set; }
    public bool? CanViewDetail { get; set; }
    public bool? Status {get; set; }
}