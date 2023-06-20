
// using AppDiv.CRVS.Application.Interfaces.Persistence;
// using AppDiv.CRVS.Domain.Entities;
// using MediatR;

// namespace AppDiv.CRVS.Application.Features.Messages.Command.Create
// {

//     public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, CreateMessageCommadResponse>
//     {
//         private readonly IMessageRepository _MessageRepository;
//         public CreateMessageCommandHandler(IMessageRepository MessageRepository)
//         {
//             _MessageRepository = MessageRepository;
//         }
//         public async Task<CreateMessageCommadResponse> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
//         {

//             // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

//             var CreateMessageCommadResponse = new CreateMessageCommadResponse();

//             var validator = new CreateMessageCommandValidator(_MessageRepository);
//             var validationResult = await validator.ValidateAsync(request, cancellationToken);

//             //Check and log validation errors
//             if (validationResult.Errors.Count > 0)
//             {
//                 CreateMessageCommadResponse.Success = false;
//                 CreateMessageCommadResponse.ValidationErrors = new List<string>();
//                 foreach (var error in validationResult.Errors)
//                     CreateMessageCommadResponse.ValidationErrors.Add(error.ErrorMessage);
//                 CreateMessageCommadResponse.Message = CreateMessageCommadResponse.ValidationErrors[0];
//             }
//             if (CreateMessageCommadResponse.Success)
//             {
//                 //can use this instead of automapper
//                 var Message = new Message
//                 {
//                     Id = Guid.NewGuid(),
//                     Key = request.Message.Key,
//                     Value = request.Message.Value,
//                     Description = request.Message.Description,
//                     StatisticCode = request.Message.StatisticCode,
//                     Code = request.Message.Code
//                 };
//                 //
//                 await _MessageRepository.InsertAsync(Message, cancellationToken);
//                 var result = await _MessageRepository.SaveChangesAsync(cancellationToken);

//                 //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
//                 // CreateMessageCommadResponse.Customer = customerResponse;          
//             }
//             return CreateMessageCommadResponse;
//         }
//     }
// }
