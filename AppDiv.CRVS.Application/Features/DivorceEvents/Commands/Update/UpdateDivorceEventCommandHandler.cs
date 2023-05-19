using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update
{

    public class UpdateDivorceEventCommandHandler : IRequestHandler<UpdateDivorceEventCommand, UpdateDivorceEventCommandResponse>
    {
        private readonly IDivorceEventRepository _DivorceEventRepository;
        private readonly IEventDocumentService _eventDocumentService;

        public UpdateDivorceEventCommandHandler(IDivorceEventRepository DivorceEventRepository, IEventDocumentService eventDocumentService)
        {
            _DivorceEventRepository = DivorceEventRepository;
            _eventDocumentService = eventDocumentService;
        }
        public async Task<UpdateDivorceEventCommandResponse> Handle(UpdateDivorceEventCommand request, CancellationToken cancellationToken)
        {

              var divorceEvent = CustomMapper.Mapper.Map<DivorceEvent>(request);
              divorceEvent.Event.EventType = "Divorce";
              await _DivorceEventRepository.InsertOrUpdateAsync(divorceEvent,true,cancellationToken);
           await _DivorceEventRepository.SaveChangesAsync(cancellationToken);
            _eventDocumentService.saveSupportingDocuments(divorceEvent.Event.EventSupportingDocuments,divorceEvent.Event.PaymentExamption.SupportingDocuments,"Divorce");
           
           return new UpdateDivorceEventCommandResponse{Message = "Divorce event Updated successfully"};
        

        }
    }
}
