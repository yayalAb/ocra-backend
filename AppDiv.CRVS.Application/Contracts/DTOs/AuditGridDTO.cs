using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities.Audit;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AuditGridDTO
    {
        public Guid? Id { get; set; }
        public string? TablePkId { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public string? AuditedEntity { get; set; }
        public string? AuditDate { get; set; }
        public Guid? AddressId { get; set; }
        public JObject? Content { get; set; }
        public string? IpAddress { get; set; }
        public AuditGridDTO(AuditLog? audit, bool withContent, IUserRepository user)
        {
            var convertor = new CustomDateConverter();
            Id = audit?.AuditId;
            TablePkId = audit?.TablePk;
            AuditDate = convertor.GregorianToEthiopic(audit!.AuditDate);
            UserName = audit?.AuditUserId != Guid.Empty ? user.GetSingle(audit!.AuditUserId.ToString())?.UserName : string.Empty;
            AuditedEntity = audit?.EntityType;
            AddressId = audit?.AddressId;
            Action = audit?.Action;
            IpAddress = audit?.AuditDataJson?.Value<string>("IpAddress");
            Content = withContent ? audit?.AuditDataJson?.Value<JObject>("ColumnValues") : null;
        }
    }
    public class SystemAuditGridDTO
    {
        public Guid? Id { get; set; }
        public string? TablePkId { get; set; }
        public string? UserName { get; set; }
        public string? Key { get; set; }
        public string? Record { get; set; }
        public string? Action { get; set; }
        public string? AuditedEntity { get; set; }
        public string? AuditDate { get; set; }
        public Guid? AddressId { get; set; }
        public SystemAuditGridDTO(AuditLog? audit, IUserRepository user)
        {
            var convertor = new CustomDateConverter();
            Id = audit?.AuditId;
            TablePkId = audit?.TablePk;
            AuditDate = convertor.GregorianToEthiopic(audit!.AuditDate);
            UserName = audit?.AuditUserId != Guid.Empty ? user.GetSingle(audit!.AuditUserId.ToString()!)?.UserName : string.Empty;
            AuditedEntity = audit?.EntityType;
            AddressId = audit?.AddressId;
            Action = audit?.Action;
            Key = audit?.EntityType == "Lookup" || audit?.EntityType == "Setting" ? audit?.AuditDataJson?.Value<JObject>("ColumnValues")?.Value<string>("Key") 
                                                : audit?.EntityType == "Address" ? ((AdminLevel)audit?.AuditDataJson?.Value<JObject>("ColumnValues")?.Value<int>("AdminLevel")!).ToString() 
                                                : null;
            var lookupValue = JObject.Parse(audit?.AuditDataJson?.Value<JObject>("ColumnValues")?.Value<string>("ValueStr")!);
            var addressName = audit?.AuditDataJson?.Value<JObject>("ColumnValues")?.Value<string?>("AddressNameStr") is not null 
                                    ? JObject.Parse(audit?.AuditDataJson?.Value<JObject>("ColumnValues")?.Value<string>("AddressNameStr")!) 
                                    : null;                                    
            Record = audit?.EntityType == "Lookup" ? "Oromiffa: " + lookupValue?.Value<string>("or") + 
                                                    ", Amharic: " + lookupValue?.Value<string>("am")
                        : audit?.EntityType == "Address" ? "Oromiffa: " + addressName?.Value<string>("or") + 
                                                          ", Amharic: " + addressName?.Value<string>("am")
                        : null;
        }
    }
    public enum AuditedEntity
    {
        BirthEvent,
        DeathEvent,
        MarriageEvent,
        DivorceEvent,
        AdoptionEvent,
        AuthenticationRequest,
        Request,
        Transaction,
        Notification,
        UserGroup,
        Setting,
        Plan,
        ApplicationUser,
        LoginHistory,
        Payment,
        PaymentRequest,
        Event,
        CourtCase,
        Court,
        Message,
        OnlineUser,
        WorkHistory,
        CorrectionRequest,
        CertificateTemplate,
        CertificateHistory,
        Address,
        Step,
        WorkFlow,
        PaymentExamption,
    }
}