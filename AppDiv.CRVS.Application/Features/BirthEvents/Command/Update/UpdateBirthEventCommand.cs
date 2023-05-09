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

        public virtual AddPersonalInfoRequest Father { get; set; }
        public virtual AddPersonalInfoRequest Mother { get; set; }
        public virtual AddEventRequest Event { get; set; }
    }

    public class UpdateBirthEventCommandHandler : IRequestHandler<UpdateBirthEventCommand, BirthEventDTO>
    {
        private readonly IBirthEventRepository _BirthEventRepository;
        public UpdateBirthEventCommandHandler(IBirthEventRepository BirthEventRepository)
        {
            _BirthEventRepository = BirthEventRepository;
        }
        public async Task<BirthEventDTO> Handle(UpdateBirthEventCommand request, CancellationToken cancellationToken)
        {
            var BirthEvent = CustomMapper.Mapper.Map<BirthEvent>(request);

            try
            {
                await _BirthEventRepository.UpdateAsync(BirthEvent, x => x.Id);
                var result = await _BirthEventRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedBirthEvent = await _BirthEventRepository.GetAsync(request.Id);
            var paymentRateResponse = CustomMapper.Mapper.Map<BirthEventDTO>(modifiedBirthEvent);

            return paymentRateResponse;
        }
    }
}