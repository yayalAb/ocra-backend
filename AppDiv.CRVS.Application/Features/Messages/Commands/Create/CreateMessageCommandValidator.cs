
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.Messages.Command.Create
{
    public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
    {
        private readonly IMessageRepository _repo;
        public CreateMessageCommandValidator(IMessageRepository repo)
        {
            _repo = repo;
          
        }


    }
}