using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Update
{
    // Customer create command with CustomerResponse
    public class UpdateDeathEventCommand : IRequest<DeathEventDTO>
    {
        public Guid Id { get; set; }
        public Guid FacilityTypeLookupId { get; set; }
        public Guid FacilityLookupId { get; set; }
        public string DuringDeath { get; set; }
        public string PlaceOfFuneral { get; set; }
        public UpdateDeathNotificationRequest DeathNotification { get; set; }
        public UpdateEventRequest Event { get; set; }
    }

    public class UpdateDeathEventCommandHandler : IRequestHandler<UpdateDeathEventCommand, DeathEventDTO>
    {
        private readonly IDeathEventRepository _deathEventRepository;
        private readonly IEventDocumentService _eventDocumentService;

        public UpdateDeathEventCommandHandler(IDeathEventRepository deathEventRepository, IEventDocumentService eventDocumentService)
        {
            this._deathEventRepository = deathEventRepository;
            this._eventDocumentService = eventDocumentService;
        }
        public async Task<DeathEventDTO> Handle(UpdateDeathEventCommand request, CancellationToken cancellationToken)
        {
            var deathEvent = CustomMapper.Mapper.Map<DeathEvent>(request);

            try
            {
                _deathEventRepository.Update(deathEvent);
                var result = await _deathEventRepository.SaveChangesAsync(cancellationToken);

                var supportingDocuments = deathEvent.Event.EventSupportingDocuments;
                var examptionDocuments = deathEvent.Event.PaymentExamption?.SupportingDocuments;

                _eventDocumentService.saveSupportingDocuments(supportingDocuments, examptionDocuments, "DeathEvents");
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedDeathEvent = await _deathEventRepository.GetIncludedAsync(request.Id);
            var paymentRateResponse = CustomMapper.Mapper.Map<DeathEventDTO>(modifiedDeathEvent);

            return paymentRateResponse;
        }
    }
}