
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Mapper;

namespace AppDiv.CRVS.Application.Features.Messages.Command.Create
{

    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, CreateMessageCommadResponse>
    {
        private readonly IMessageRepository _MessageRepository;
        public CreateMessageCommandHandler(IMessageRepository MessageRepository)
        {
            _MessageRepository = MessageRepository;
        }
        public async Task<CreateMessageCommadResponse> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {

            var CreateMessageCommadResponse = new CreateMessageCommadResponse();

            var validator = new CreateMessageCommandValidator(_MessageRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                CreateMessageCommadResponse.Success = false;
                CreateMessageCommadResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateMessageCommadResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateMessageCommadResponse.Message = CreateMessageCommadResponse.ValidationErrors[0];
            }
            if (CreateMessageCommadResponse.Success)
            {
                var newMessage = CustomMapper.Mapper.Map<Message>(request);
                await _MessageRepository.InsertAsync(newMessage, cancellationToken);
                 await _MessageRepository.SaveChangesAsync(cancellationToken);
                 CreateMessageCommadResponse.CreatedMessage = newMessage;
                 CreateMessageCommadResponse.Message = "message created successful";
            }
            return CreateMessageCommadResponse;
        }
    }
}
