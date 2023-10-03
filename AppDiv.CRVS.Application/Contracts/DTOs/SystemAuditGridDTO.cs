using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Entities.Audit;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class SystemAuditGridDTO
    {
        public Guid? Id { get; set; }
        public string? TablePkId { get; set; }
        public string? UserName { get; set; }
        public string? UserFullName { get; set; }
        public string? UserId { get; set; }
        public string? Key { get; set; }
        public string? Record { get; set; }
        public string? Action { get; set; }
        public string? AuditedEntity { get; set; }
        public string? AuditDate { get; set; }
        public string? Address { get; set; }
        public SystemAuditGridDTO(AuditLog? audit)
        {
            var convertor = new CustomDateConverter();
            Id = audit?.AuditId;
            TablePkId = audit?.TablePk;
            AuditDate = convertor.GregorianToEthiopic(audit!.AuditDate);
            UserName = audit?.AuditUser.UserName;
            UserId = audit?.AuditUserId;
            UserFullName = audit?.AuditUser?.PersonalInfo?.FullNameLang;
            AuditedEntity = audit?.EntityType;
            Address = $"{audit?.Address?.ParentAddress?.ParentAddress?.AddressNameLang}/{audit?.Address?.ParentAddress?.AddressNameLang}/{audit?.Address?.AddressNameLang}".Trim('/');
            Action = audit?.Action;
            Key = audit?.EntityType == "Lookup" || audit?.EntityType == "Setting" ? audit?.AuditDataJson?.Value<JObject>("ColumnValues")?.Value<string>("Key") 
                                                : audit?.EntityType == "Address" ? ((AdminLevel)audit?.AuditDataJson?.Value<JObject>("ColumnValues")?.Value<int>("AdminLevel")!).ToString() 
                                                : null;
            var lookup = audit?.AuditDataJson?.Value<JObject>("ColumnValues")?.ToObject<Lookup>();
            var address = audit?.AuditDataJson?.Value<JObject>("ColumnValues")?.ToObject<Address>();                           
            Record = audit?.EntityType == "Lookup" ? lookup?.ValueLang
                        : audit?.EntityType == "Address" ? address?.AddressNameLang : null;
        }
    }
}