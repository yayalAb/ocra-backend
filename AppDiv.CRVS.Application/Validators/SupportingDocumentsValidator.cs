using AppDiv.CRVS.Application.Service;
using FluentValidation;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Validators
{
    public class SupportingDocumentsValidator : AbstractValidator<ICollection<AddSupportingDocumentRequest>>
    {
        public SupportingDocumentsValidator()
        {
            RuleFor(p => p).NotEmpty().NotNull().SupportingDocNull("Event.EventSupportingDocuments");
        }
    }
}