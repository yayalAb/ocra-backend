using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{

    public class CreateBirthEventCommandHandler : IRequestHandler<CreateBirthEventCommand, CreateBirthEventCommandResponse>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        private readonly IFileService _fileService;
        public CreateBirthEventCommandHandler(IBirthEventRepository birthEventRepository, IFileService fileService)
        {
            this._fileService = fileService;
            _birthEventRepository = birthEventRepository;
        }
        public async Task<CreateBirthEventCommandResponse> Handle(CreateBirthEventCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var createPaymentCommandResponse = new CreateBirthEventCommandResponse();

            var validator = new CreateBirthEventCommandValidator(_birthEventRepository);
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

                var BirthEvent = CustomMapper.Mapper.Map<BirthEvent>(request.BirthEvent);
                BirthEvent.Event.EventType = "BirthEvent";

                await _birthEventRepository.InsertOrUpdateAsync(BirthEvent, cancellationToken);
                var result = await _birthEventRepository.SaveChangesAsync(cancellationToken);

                var files = request.BirthEvent.Event.EventSupportingDocuments.Select(doc => doc.base64String).ToList();
                var fileNames = request.BirthEvent.Event.EventSupportingDocuments.Select(doc => doc.Id).ToList();
                var folderName = Path.Combine("Resources", "SupportingDocuments", "BirthEvents");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                // await _fileService.UploadBase64FilesAsync(files, fileNames, pathToSave, FileMode.Create);

                var paymentExamption = request.BirthEvent.Event.PaymentExamption.SupportingDocuments.Select(doc => doc.base64String).ToList();
                var paymentExamptionIds = request.BirthEvent.Event.PaymentExamption.SupportingDocuments.Select(doc => doc.Id).ToList();
                // await _fileService.UploadBase64FilesAsync(paymentExamption, paymentExamptionIds, pathToSave, FileMode.Create);

                var allDocs = files.Concat(paymentExamption).ToList();
                var allNames = fileNames.Concat(paymentExamptionIds).ToList();
                await _fileService.UploadBase64FilesAsync(allDocs, allNames, pathToSave, FileMode.Create);

                // if (paymentExamption != null)
                // {
                //     string examptionPath = Path.Combine(pathToSave, "PaymentExamptions");
                //     await _fileService.UploadBase64FileAsync(paymentExamption,
                //                                 request.BirthEvent.Event.EventPaymentExamptionNavigation.Id.ToString(),
                //                                 examptionPath,
                //                                 FileMode.Create);
                // }
                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // createCustomerCommandResponse.Customer = customerResponse;          
            }
            return createPaymentCommandResponse;
        }
    }
}
