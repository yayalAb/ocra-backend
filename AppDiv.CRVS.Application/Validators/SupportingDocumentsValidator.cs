using AppDiv.CRVS.Application.Service;
using FluentValidation;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Validators
{
    public class SupportingDocumentsValidator : AbstractValidator<ICollection<AddSupportingDocumentRequest>>
    {
        public SupportingDocumentsValidator(string propertyName)
        {
            RuleFor(p => p).SupportingDocNull(propertyName);
            
        }
    }
}