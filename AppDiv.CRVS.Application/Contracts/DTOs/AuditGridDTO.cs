using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities.Audit;
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
        public string Address { get; set; }
        public JObject? Content { get; set; }
        public AuditGridDTO(AuditLog? audit, bool withContent)
        {
            Id = audit?.AuditId;
            TablePkId = audit?.TablePk;
            UserName = audit?.EnviromentJson?.Value<string>("UserName");
            AuditedEntity = audit?.EntityType;
            Action = audit?.Action;
            Content = withContent ? audit?.AuditDataJson?.Value<JObject>("ColumnValues") : null;
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