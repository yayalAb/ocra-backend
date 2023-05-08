using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Create
{

    public class CreateDeathEventCommandHandler : IRequestHandler<CreateDeathEventCommand, CreateDeathEventCommandResponse>
    {
        private readonly IDeathEventRepository _deathEventRepository;
        private readonly IFileService _fileService;
        public CreateDeathEventCommandHandler(IDeathEventRepository deathEventRepository, IFileService fileService)
        {
            this._fileService = fileService;
            _deathEventRepository = deathEventRepository;
        }
        public async Task<CreateDeathEventCommandResponse> Handle(CreateDeathEventCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var createPaymentCommandResponse = new CreateDeathEventCommandResponse();

            var validator = new CreateDeathEventCommandValidator(_deathEventRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                createPaymentCommandResponse.Success = false;
                createPaymentCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    createPaymentCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                createPaymentCommandResponse.Message = createPaymentCommandResponse.ValidationErrors[0];
            }
            if (createPaymentCommandResponse.Success)
            {
                // var docs = await _groupRepository.GetMultipleUserGroups(request.UserGroups);

                var deathEvent = CustomMapper.Mapper.Map<DeathEvent>(request.DeathEvent);
                deathEvent.Event.EventType = "DeathEvent";

                await _deathEventRepository.InsertAsync(deathEvent, cancellationToken);
                var result = await _deathEventRepository.SaveChangesAsync(cancellationToken);

                var files = request.DeathEvent.Event.Attachments.Select(doc => doc.FileStr).ToList();
                var fileNames = request.DeathEvent.Event.Attachments.Select(doc => doc.Id).ToList();
                var folderName = Path.Combine("Resources", "SupportingDocuments", "DeathEvents");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                await _fileService.UploadBase64FilesAsync(files, fileNames, pathToSave, FileMode.Create);

                var paymentExamption = request.DeathEvent.Event.PaymentExamption.Document;
                if (paymentExamption != null)
                {
                    string examptionPath = Path.Combine(pathToSave, "PaymentExamptions");
                    await _fileService.UploadBase64FileAsync(paymentExamption,
                                                request.DeathEvent.Event.PaymentExamption.ExamptionRequestId.ToString(),
                                                examptionPath,
                                                FileMode.Create);
                }
                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // createCustomerCommandResponse.Customer = customerResponse;          
            }
            return createPaymentCommandResponse;
        }
    }
}
