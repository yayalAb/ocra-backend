using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class EventDTO
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public Guid EventOwenerId { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventRegDate { get; set; }
        public Guid EventAddressId { get; set; }
        public PersonalInfoDTO RegistrarInfo { get; set; }
        public PaymentExamptionDTO PaymentExamption { get; set; }
        public Guid InformantTypeLookupId { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public bool IsExampted { get; set; } = false;
        public bool IsCertified { get; set; } = false;
        public PersonalInfoDTO EventOwener { get; set; }

        public ICollection<SupportingDocumentDTO> Attachments { get; set; }
    }
}