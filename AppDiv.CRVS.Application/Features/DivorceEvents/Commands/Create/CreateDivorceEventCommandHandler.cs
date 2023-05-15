using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create
{

    public class CreateDivorceEventCommandHandler : IRequestHandler<CreateDivorceEventCommand, CreateDivorceEventCommandResponse>
    {
        private readonly IDivorceEventRepository _DivorceEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IEventDocumentService _eventDocumentService;

        public CreateDivorceEventCommandHandler(IDivorceEventRepository DivorceEventRepository, IPersonalInfoRepository personalInfoRepository, IEventDocumentService eventDocumentService)
        {
            _DivorceEventRepository = DivorceEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _eventDocumentService = eventDocumentService;
        }
        public async Task<CreateDivorceEventCommandResponse> Handle(CreateDivorceEventCommand request, CancellationToken cancellationToken)
        {
            var divorceEvent = CustomMapper.Mapper.Map<DivorceEvent>(request);
            divorceEvent.Event.EventType = "Divorce";
           await _DivorceEventRepository.InsertOrUpdateAsync(divorceEvent, cancellationToken);
           await _DivorceEventRepository.SaveChangesAsync(cancellationToken);
            _eventDocumentService.saveSupportingDocuments(divorceEvent.Event.EventSupportingDocuments,divorceEvent.Event.PaymentExamption.SupportingDocuments,"Divorce");
           
           return new CreateDivorceEventCommandResponse{Message = "Divorce event created successfully"};
        }
    }
}
