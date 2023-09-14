using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Entities.Audit;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs;
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
            UserName = audit?.AuditUserId != string.Empty ? user.GetSingle(audit!.AuditUserId!.ToString()!)?.UserName : string.Empty;
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
        public string? Address { get; set; }
        public SystemAuditGridDTO(AuditLog? audit)
        {
            var convertor = new CustomDateConverter();
            Id = audit?.AuditId;
            TablePkId = audit?.TablePk;
            AuditDate = convertor.GregorianToEthiopic(audit!.AuditDate);
            UserName = audit?.AuditUser.UserName;
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
    public class WorkHistoryAuditGridDTO
    {
        public Guid? Id { get; set; }
        public string? UserName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Roles { get; set; }
        public string? Address { get; set; }
        public WorkHistoryAuditGridDTO(WorkHistory? history)
        {
            var convertor = new CustomDateConverter();
            Id = history?.Id;
            UserName = history?.User.UserName;
            StartDate = convertor.GregorianToEthiopic(history!.StartDate);
            EndDate = convertor.GregorianToEthiopic(history!.CreatedAt);
            Roles = string.Join(", ",history?.UserGroups?.Select(g => g.GroupName)!);   
            Address = $"{history?.Address?.ParentAddress?.ParentAddress?.AddressNameLang}/{history?.Address?.ParentAddress?.AddressNameLang}/{history?.Address?.AddressNameLang}".Trim('/');
        }
    }

    public class LoginAuditGridDTO
    {
        public Guid? Id { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public string? IpAddress { get; set; }
        public string? AuditDate { get; set; }
        public string? Address { get; set; }
        public LoginAuditGridDTO(LoginHistory? history)
        {
            var convertor = new CustomDateConverter();
            Id = history?.Id;
            AuditDate = history?.EventDate is not null ? convertor.GregorianToEthiopic((DateTime)history.EventDate) : null;
            UserName = history?.User.UserName;
            Address = $"{history?.User?.Address?.ParentAddress?.ParentAddress?.AddressNameLang}/{history?.User?.Address?.ParentAddress?.AddressNameLang}/{history?.User?.Address?.AddressNameLang}".Trim('/');
            Action = history?.EventType;
            IpAddress = history?.IpAddress;
            
        }
    }

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

    public class TransactionAuditGridDTO
    {
        public Guid? Id { get; set; }
        public string? RequestedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public string? RequestDate { get; set; }
        public string? ApprovalDate { get; set; }
        public string? RequestType { get; set; }
        public bool? RequestStatus { get; set; }
        public int? CurrentStep { get; set; }
        public string? StepName { get; set; }
        public string? CertificateId { get; set; }
        public TransactionAuditGridDTO(Transaction? transaction)
        {
            var convertor = new CustomDateConverter();
            Id = transaction?.Id;
            RequestedBy = $"{transaction!.Request?.CivilRegOfficer.FirstNameLang} {transaction!.Request!.CivilRegOfficer.MiddleNameLang} {transaction.Request.CivilRegOfficer.LastNameLang}";
            ApprovedBy = $"{transaction.CivilRegOfficer?.PersonalInfo.FirstNameLang} {transaction.CivilRegOfficer?.PersonalInfo.MiddleNameLang} {transaction.CivilRegOfficer?.PersonalInfo.LastNameLang}";
            RequestDate = convertor.GregorianToEthiopic(transaction.Request.CreatedAt);
            ApprovalDate = convertor.GregorianToEthiopic(transaction.CreatedAt);
            RequestType = transaction.Request.RequestType;
            RequestStatus = transaction.ApprovalStatus;
            CurrentStep = transaction.CurrentStep;
            StepName = transaction!.Workflow!.Steps.Where(s => s.step == transaction.CurrentStep).Select(s => s.DescriptionLang.ToString()).SingleOrDefault();
            CertificateId = transaction.Request?.AuthenticationRequest?.Certificate?.Event?.CertificateId ??
                            transaction.Request?.CorrectionRequest?.Event.CertificateId ??
                            transaction.Request?.VerficationRequest?.Event.CertificateId ??
                            transaction.Request?.PaymentRequest?.Event.CertificateId;
        }
    }