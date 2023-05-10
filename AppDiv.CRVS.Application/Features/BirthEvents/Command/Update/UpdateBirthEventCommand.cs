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
        public virtual AddEventRequest Event { get; set; }
        public virtual AddBirthNotificationRequest BirthNotification { get; set; }
    }

    public class UpdateBirthEventCommandHandler : IRequestHandler<UpdateBirthEventCommand, BirthEventDTO>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        public UpdateBirthEventCommandHandler(IBirthEventRepository birthEventRepository)
        {
            _birthEventRepository = birthEventRepository;
        }
        public async Task<BirthEventDTO> Handle(UpdateBirthEventCommand request, CancellationToken cancellationToken)
        {
            var birthEvent = CustomMapper.Mapper.Map<BirthEvent>(request);

            try
            {
                await _birthEventRepository.UpdateAsync(birthEvent, x => x.Id);
                var result = await _birthEventRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedBirthEvent = await _birthEventRepository.GetAsync(request.Id);
            var paymentRateResponse = CustomMapper.Mapper.Map<BirthEventDTO>(modifiedBirthEvent);

            return paymentRateResponse;
        }
    }
}