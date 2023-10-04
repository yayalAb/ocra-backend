
using System.Text.RegularExpressions;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.User.Command.Update
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly IUserRepository _repo;
        public UpdateUserCommandValidator(IUserRepository repo)
        {
            _repo = repo;
            // RuleFor(n => n.UserImage)
            // .NotEmpty()
            // .NotNull()
            // .Must(isValidBase64String).WithMessage("user Image is invalid base64String");

        }
        private bool isValidBase64String(string? base64String)
        {
            if (base64String == null)
            {
                return true;
            }
            try
            {
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                base64String = regex.Replace(base64String, string.Empty);

                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}