using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
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

namespace AppDiv.CRVS.Application.Features.DeathEvents.Query
{
    // Customer query with List<Customer> response
    public record GetAllDeathEventQuery : IRequest<PaginatedList<DeathEventDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllDeathEventHandler : IRequestHandler<GetAllDeathEventQuery, PaginatedList<DeathEventDTO>>
    {
        private readonly IDeathEventRepository _deathEventRepository;

        public GetAllDeathEventHandler(IDeathEventRepository deathEventQueryRepository)
        {
            _deathEventRepository = deathEventQueryRepository;
        }
        public async Task<PaginatedList<DeathEventDTO>> Handle(GetAllDeathEventQuery request, CancellationToken cancellationToken)
        {

            // var paymentRateList = await _paymentRateRepository.GetAll(new string[] { "PaymentTypeLookup", "EventLookup", "Address" });
            return await PaginatedList<DeathEventDTO>
                            .CreateAsync(
                                _deathEventRepository.GetAll().Select(de => new DeathEventDTO
                                {
                                    Id = de.Id,
                                    FacilityType = CustomMapper.Mapper.Map<LookupDTO>(de.FacilityType),
                                    Facility = CustomMapper.Mapper.Map<LookupDTO>(de.Facility),
                                    DuringDeath = de.DuringDeath,
                                    PlaceOfFuneral = de.PlaceOfFuneral,
                                    DeathNotification = CustomMapper.Mapper.Map<DeathNotificationDTO>(de.DeathNotification),
                                    Event = CustomMapper.Mapper.Map<EventDTO>(de.Event),
                                }).ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
            // var paymentRateResponse = CustomMapper.Mapper.Map<List<DeathEventDTO>>(paymentRateList);
            // return paymentRateResponse;
        }
    }
}