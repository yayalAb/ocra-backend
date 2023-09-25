using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities.Audit;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class EventAuditGridDTO
    {
        public Guid? Id { get; set; }
        public string? TablePkId { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public string? AuditedEntity { get; set; }
        public string? AuditDate { get; set; }
        public string? Address { get; set; }
        public string? CertificateId { get; set; }
        public EventAuditGridDTO(AuditLog? audit, IEventRepository eventRepository)
        {
            var convertor = new CustomDateConverter();
            Id = audit?.AuditId;
            TablePkId = audit?.TablePk;
            AuditDate = convertor.GregorianToEthiopic(audit!.AuditDate);
            UserName = audit?.AuditUser?.UserName;
            AuditedEntity = audit?.EntityType;
            Address = $"{audit?.Address?.ParentAddress?.ParentAddress?.AddressNameLang}/{audit?.Address?.ParentAddress?.AddressNameLang}/{audit?.Address?.AddressNameLang}".Trim('/');
            Action = audit?.Action;
            CertificateId = eventRepository.GetSingle(Guid.Parse(audit?.AuditDataJson?.Value<JObject>("ColumnValues")?.Value<string>("EventId")!))?.CertificateId;
        }
    }
}