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
        private readonly IEventDocumentService _eventDocumentService;

        public UpdateMarriageEventCommandHandler(IMarriageEventRepository marriageEventRepository, IPersonalInfoRepository personalInfoRepository, IEventDocumentService eventDocumentService)
        {
            _marriageEventRepository = marriageEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _eventDocumentService = eventDocumentService;
        }
        public async Task<UpdateMarriageEventCommandResponse> Handle(UpdateMarriageEventCommand request, CancellationToken cancellationToken)
        {
            var marriageEvent = CustomMapper.Mapper.Map<MarriageEvent>(request);
            marriageEvent.Event.EventType = "Marriage";
            _marriageEventRepository.EFUpdate(marriageEvent);
            await _marriageEventRepository.SaveChangesAsync(cancellationToken);

            var eventSupportingDocuments = marriageEvent.Event.EventSupportingDocuments;
            var examptionSupportingDocuments = marriageEvent.Event?.PaymentExamption?.SupportingDocuments;
            _eventDocumentService.saveSupportingDocuments(eventSupportingDocuments,examptionSupportingDocuments, "Marriage");


            return new UpdateMarriageEventCommandResponse { Message = "Marriage Event Updated Successfully" };


        }
    }
}
