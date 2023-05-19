using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Validators
{
    public class PaymentExamptionValidator : AbstractValidator<AddPaymentExamptionRequest>
    {
        private readonly IPaymentExamptionRequestRepository _repo;
        public PaymentExamptionValidator(IPaymentExamptionRequestRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.ExamptionRequestId.ToString()).NotGuidEmpty().ForeignKeyWithPaymentExamptionRequest(_repo, "Event.PaymentExamption.ExamptionRequestId");
            RuleFor(p => p.SupportingDocuments).SupportingDocNull("Event.PaymentExamption.EventSupportingDocuments").NotEmpty().NotNull();
        }
    }
}