using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
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
        public Guid FacilityTypeId { get; set; }
        public Guid FacilityId { get; set; }
        public string DuringDeath { get; set; }
        public string PlaceOfFuneral { get; set; }
        public AddDeathNotificationRequest DeathNotification { get; set; }
        public AddEventRequest Event { get; set; }
    }

    public class UpdateDeathEventCommandHandler : IRequestHandler<UpdateDeathEventCommand, DeathEventDTO>
    {
        private readonly IDeathEventRepository _deathEventRepository;
        public UpdateDeathEventCommandHandler(IDeathEventRepository deathEventRepository)
        {
            _deathEventRepository = deathEventRepository;
        }
        public async Task<DeathEventDTO> Handle(UpdateDeathEventCommand request, CancellationToken cancellationToken)
        {
            var deathEvent = CustomMapper.Mapper.Map<DeathEvent>(request);

            // var deathEvent = new DeathEvent()
            // {
            //     Id = request.Id,
            //     // PaymentTypeLookupId = request.PaymentTypeLookupId,
            //     // EventLookupId = request.EventLookupId,
            //     // AddressId = request.AddressId,
            //     // Amount = request.Amount,
            //     // Status = request.Status,
            //     // ModifiedAt = DateTime.Now
            // };


            try
            {
                await _deathEventRepository.UpdateAsync(deathEvent, x => x.Id);
                var result = await _deathEventRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedDeathEvent = await _deathEventRepository.GetAsync(request.Id);
            var paymentRateResponse = CustomMapper.Mapper.Map<DeathEventDTO>(modifiedDeathEvent);

            return paymentRateResponse;
        }
    }
}