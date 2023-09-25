using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
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
}