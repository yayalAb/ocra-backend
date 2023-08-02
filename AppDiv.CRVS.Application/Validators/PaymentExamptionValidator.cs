using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Validators
{
    public class PaymentExamptionValidator : AbstractValidator<AddPaymentExamptionRequest>
    {
        private readonly IEventRepository _repo;
        public PaymentExamptionValidator(IEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p).NotNull().NotEmpty().WithMessage("Payment Examption Can not be empty or null if the is exampted is true.");
            RuleFor(p => p.ExamptionReasonLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "Event.PaymentExamption.ExamptionReasonLookupId");
            RuleFor(p => p.SupportingDocuments).SupportingDocNull("Event.PaymentExamption.EventSupportingDocuments")
            .When(p => p.SupportingDocuments != null);
        }
    }
}