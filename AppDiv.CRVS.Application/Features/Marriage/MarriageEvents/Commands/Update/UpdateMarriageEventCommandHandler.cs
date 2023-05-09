using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update
{

    public class UpdateMarriageEventCommandHandler : IRequestHandler<UpdateMarriageEventCommand, UpdateMarriageEventCommandResponse>
    {
        private readonly IMarriageEventRepository _marriageEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IFileService _fileService;

        public UpdateMarriageEventCommandHandler(IMarriageEventRepository marriageEventRepository, IPersonalInfoRepository personalInfoRepository, IFileService fileService)
        {
            _marriageEventRepository = marriageEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _fileService = fileService;
        }
        public async Task<UpdateMarriageEventCommandResponse> Handle(UpdateMarriageEventCommand request, CancellationToken cancellationToken)
        {
            var marriageEvent = CustomMapper.Mapper.Map<MarriageEvent>(request);
            //TODO:override insert function to add for the above conditions
             _marriageEventRepository.EFUpdate(marriageEvent);
            await _marriageEventRepository.SaveChangesAsync(cancellationToken);

            var eventSupportingDocuments = marriageEvent.Event.EventSupportingDocuments;
            var examptionSupportingDocuments = marriageEvent.Event.PaymentExamption.SupportingDocuments;
            var supportingDocFolder = Path.Combine("Resources", "SupportingDocuments", "Marriage");
            var examptiondocFolder = Path.Combine("Resources", "ExamptionDocuments", "Marriage");
            var fullPathSupporting = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);
            var fullPathExamption = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);


            eventSupportingDocuments.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathSupporting, FileMode.Create);
            });
            examptionSupportingDocuments.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathExamption, FileMode.Create);
            });

            return new UpdateMarriageEventCommandResponse{Message = "Marriage Event Updated Successfully"};

            // var executionStrategy = _marriageEventRepository.Datab.UpdateExecutionStrategy();
            // return await executionStrategy.ExecuteAsync(async () =>
            // {

            //     using (var transaction = _context.database.BeginTransaction())
            //     {

            //         try

            //         {
            //             await _context.Companies.AddAsync(_mapper.Map<Company>(request));
            //             await _context.SaveChangesAsync(cancellationToken);
            //             await transaction.CommitAsync();

            //             return CustomResponse.Succeeded("Company Updated Successfully", 201);

            //         }
            //         catch (Exception)
            //         {
            //             await transaction.RollbackAsync();
            //             throw;
            //         }
            //     }

            // });


        }
    }
}
