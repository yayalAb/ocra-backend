using FluentValidation;

namespace AppDiv.CRVS.Application.Features.Auth.ForgotPassword
{
    internal class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(u => u.Email)
             .NotEmpty()
             .NotNull()
             .EmailAddress();
        }
    }
}
