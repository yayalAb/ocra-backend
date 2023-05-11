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

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Update
{
    // Customer create command with CustomerResponse
    public class UpdateBirthEventCommand : IRequest<BirthEventDTO>
    {
        public Guid Id { get; set; }
        public Guid FatherId { get; set; }
        public Guid MotherId { get; set; }
        public Guid FacilityTypeId { get; set; }
        public Guid FacilityId { get; set; }
        public Guid BirthPlaceId { get; set; }
        public Guid TypeOfBirthId { get; set; }
        public Guid EventId { get; set; }

        public virtual UpdatePersonalInfoRequest Father { get; set; }
        public virtual UpdatePersonalInfoRequest Mother { get; set; }
        public virtual UpdateEventRequest Event { get; set; }
        public virtual UpdateBirthNotificationRequest BirthNotification { get; set; }
    }

    public class UpdateBirthEventCommandHandler : IRequestHandler<UpdateBirthEventCommand, BirthEventDTO>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        public UpdateBirthEventCommandHandler(IBirthEventRepository birthEventRepository, IEventDocumentService eventDocumentService)
        {
            this._eventDocumentService = eventDocumentService;
            _birthEventRepository = birthEventRepository;
        }
        public async Task<BirthEventDTO> Handle(UpdateBirthEventCommand request, CancellationToken cancellationToken)
        {
            var birthEvent = CustomMapper.Mapper.Map<BirthEvent>(request);

            try
            {
                _birthEventRepository.Update(birthEvent);
                var result = await _birthEventRepository.SaveChangesAsync(cancellationToken);

                var supportingDocuments = birthEvent.Event.EventSupportingDocuments;
                var examptionDocuments = birthEvent.Event.PaymentExamption?.SupportingDocuments;

                _eventDocumentService.saveSupportingDocuments(supportingDocuments, examptionDocuments, "BirthEvents");
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedBirthEvent = await _birthEventRepository.GetWithIncludedAsync(request.Id);
            var paymentRateResponse = CustomMapper.Mapper.Map<BirthEventDTO>(modifiedBirthEvent);

            return paymentRateResponse;
        }
    }
}