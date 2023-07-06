using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class EventDTO
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string? RegBookNo { get; set; }
        public string? CivilRegOfficeCode { get; set; }
        public string CertificateId { get; set; }
        // public Guid EventOwenerId { get; set; }
        // public DateTime EventDate { get; set; }
        public string EventDateEt { get; set; }
        // public DateTime EventRegDate { get; set; }
        public string EventRegDateEt { get; set; }
        public Guid EventAddressId { get; set; }
        public AddressResponseDTOE? EventAddress { get; set; }
        public RegistrarDTO EventRegistrar { get; set; }
        public PaymentExamptionDTO PaymentExamption { get; set; }
        public string? InformantType { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public bool IsExampted { get; set; } = false;
        public bool IsPaid { get; set; } = false;

        public bool IsCertified { get; set; } = false;
        public UpdatePersonalInfoRequest EventOwener { get; set; }

        public ICollection<SupportingDocumentDTO> EventSupportingDocuments { get; set; }
    }
}