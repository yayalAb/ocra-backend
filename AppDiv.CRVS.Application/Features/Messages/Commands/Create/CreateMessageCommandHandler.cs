
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Utility.Services;
using Microsoft.EntityFrameworkCore;



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
                var message = await _MessageRepository.GetAll()
                        .Include(m => m.Sender)
                        .Include(m => m.Receiver)
                        .Where(m => m.Id == newMessage.Id).FirstOrDefaultAsync();
                CreateMessageCommadResponse.CreatedMessage = new MessageDTO
                {
                    Id = message.Id,
                    Type = message.Type,
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    TextMessage = message.TextMessage,
                    ParentMessageId = message.ParentMessageId,
                    SenderName = message.Sender.UserName,
                    ReceiverName = message.Receiver.UserName,
                    CreatedAt = message.CreatedAt,
                    CreatedAtEt = message.CreatedAt == null ? "" : new CustomDateConverter(message.CreatedAt).ethiopianDate

                };
                CreateMessageCommadResponse.Message = "message created successful";
            }
            return CreateMessageCommadResponse;
        }
    }
}
