using FluentValidation;

namespace Application.User.Commands.ForgotPassword
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
