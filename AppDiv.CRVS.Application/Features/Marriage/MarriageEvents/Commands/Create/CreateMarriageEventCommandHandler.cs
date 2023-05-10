
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create
{

    public class CreateMarriageEventCommandHandler : IRequestHandler<CreateMarriageEventCommand, CreateMarriageEventCommandResponse>
    {
        private readonly IMarriageEventRepository _marriageEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IEventDocumentService _eventDocumentService;

        public CreateMarriageEventCommandHandler(IMarriageEventRepository marriageEventRepository, IPersonalInfoRepository personalInfoRepository, IEventDocumentService eventDocumentService)
        {
            _marriageEventRepository = marriageEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _eventDocumentService = eventDocumentService;
        }
        public async Task<CreateMarriageEventCommandResponse> Handle(CreateMarriageEventCommand request, CancellationToken cancellationToken)
        {


            var CreateMarriageEventCommandResponse = new CreateMarriageEventCommandResponse();

            var validator = new CreateMarriageEventCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                CreateMarriageEventCommandResponse.Success = false;
                CreateMarriageEventCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateMarriageEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateMarriageEventCommandResponse.Message = CreateMarriageEventCommandResponse.ValidationErrors[0];
            }
            if (CreateMarriageEventCommandResponse.Success)
            {

            var marriageEvent = CustomMapper.Mapper.Map<MarriageEvent>(request);
            await _marriageEventRepository.InsertOrUpdateAsync(marriageEvent, cancellationToken);
            await _marriageEventRepository.SaveChangesAsync(cancellationToken);
            _eventDocumentService.saveSupportingDocuments(marriageEvent.Event.EventSupportingDocuments, marriageEvent.Event.PaymentExamption.SupportingDocuments, "Marriage");


            // var executionStrategy = _marriageEventRepository..CreateExecutionStrategy();
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
            // return new CreateMarriageEventCommandResponse { Message = "Marriage Event created Successfully" };
            return CreateMarriageEventCommandResponse;

        }
    }
}
