using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create
{

    public class CreateMarriageEventCommandHandler : IRequestHandler<CreateMarriageEventCommand, CreateMarriageEventCommandResponse>
    {
        private readonly IMarriageEventRepository _marriageEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IFileService _fileService;

        public CreateMarriageEventCommandHandler(IMarriageEventRepository marriageEventRepository, IPersonalInfoRepository personalInfoRepository, IFileService fileService)
        {
            _marriageEventRepository = marriageEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _fileService = fileService;
        }
        public async Task<CreateMarriageEventCommandResponse> Handle(CreateMarriageEventCommand request, CancellationToken cancellationToken)
        {
            var marriageEvent = CustomMapper.Mapper.Map<MarriageEvent>(request);
            if (marriageEvent.BrideInfo.Id != null && marriageEvent.BrideInfo.Id != Guid.Empty)
            {
                _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(marriageEvent.BrideInfo));
                marriageEvent.BrideInfoId = marriageEvent.BrideInfo.Id;
                marriageEvent.BrideInfo = null;
            }
            if (marriageEvent.Event.EventOwener.Id != null && marriageEvent.Event.EventOwener.Id != Guid.Empty)
            {
                _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(marriageEvent.Event.EventOwener));
                marriageEvent.Event.EventOwenerId = marriageEvent.Event.EventOwener.Id;
                marriageEvent.Event.EventOwener = null;
            }
            if (marriageEvent.Event.EventRegistrar.RegistrarInfo.Id != null && marriageEvent.Event.EventRegistrar.RegistrarInfo.Id != Guid.Empty)
            {
                _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(marriageEvent.Event.EventRegistrar.RegistrarInfo));
                marriageEvent.Event.EventRegistrar.RegistrarInfoId = marriageEvent.Event.EventRegistrar.RegistrarInfo.Id;
                marriageEvent.Event.EventRegistrar.RegistrarInfo = null;
            }

            marriageEvent.Witnesses?.ToList().ForEach(async witness =>
            {
                if (witness.WitnessPersonalInfo.Id != null)
                {
                    _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(witness.WitnessPersonalInfo));
                    witness.WitnessPersonalInfoId = witness.WitnessPersonalInfo.Id;
                    witness.WitnessPersonalInfo = null;
                }
            });
            //TODO:override insert function to add for the above conditions

            await _marriageEventRepository.InsertAsync(marriageEvent, cancellationToken);
            await _marriageEventRepository.SaveChangesAsync(cancellationToken);

            var eventSupportingDocuments = marriageEvent.Event.EventSupportingDocuments;
            var examptionSupportingDocuments = marriageEvent.Event.PaymentExamption?.SupportingDocuments;
            var supportingDocFolder = Path.Combine("Resources", "SupportingDocuments", "Marriage");
            var examptiondocFolder = Path.Combine("Resources", "ExamptionDocuments", "Marriage");
            var fullPathSupporting = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);
            var fullPathExamption = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);


            eventSupportingDocuments?.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathSupporting, FileMode.Create);
            });
            examptionSupportingDocuments?.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathExamption, FileMode.Create);
            });

            return new CreateMarriageEventCommandResponse{Message = "Marriage Event created Successfully"};

            // var executionStrategy = _marriageEventRepository.Datab.CreateExecutionStrategy();
            // return await executionStrategy.ExecuteAsync(async () =>
            // {

            //     using (var transaction = _context.database.BeginTransaction())
            //     {

            //         try

            //         {
            //             await _context.Companies.AddAsync(_mapper.Map<Company>(request));
            //             await _context.SaveChangesAsync(cancellationToken);
            //             await transaction.CommitAsync();

            //             return CustomResponse.Succeeded("Company Created Successfully", 201);

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
