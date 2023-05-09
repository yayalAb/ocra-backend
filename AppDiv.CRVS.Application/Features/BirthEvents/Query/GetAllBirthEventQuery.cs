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

namespace AppDiv.CRVS.Application.Features.BirthEvents.Query
{
    // Customer query with List<Customer> response
    public record GetAllBirthEventQuery : IRequest<PaginatedList<BirthEventDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllBirthEventHandler : IRequestHandler<GetAllBirthEventQuery, PaginatedList<BirthEventDTO>>
    {
        private readonly IBirthEventRepository _paymentRateRepository;

        public GetAllBirthEventHandler(IBirthEventRepository paymentRateQueryRepository)
        {
            _paymentRateRepository = paymentRateQueryRepository;
        }
        public async Task<PaginatedList<BirthEventDTO>> Handle(GetAllBirthEventQuery request, CancellationToken cancellationToken)
        {

            // var paymentRateList = await _paymentRateRepository.GetAll(new string[] { "PaymentTypeLookup", "EventLookup", "Address" });
            return await PaginatedList<BirthEventDTO>
                            .CreateAsync(
                                _paymentRateRepository.GetAll().Select(de => new BirthEventDTO
                                {
                                    Id = de.Id,
                                    FatherId = de.FatherId,           // Father = CustomMapper.Mapper.Map<PersonalInfoDTO>(de.Father),
                                    MotherId = de.MotherId,           // Father = CustomMapper.Mapper.Map<PersonalInfoDTO>(de.Father),
                                    // Mother = CustomMapper.Mapper.Map<PersonalInfoDTO>(de.Mother),
                                    // BirthPlace = CustomMapper.Mapper.Map<AddressDTO>(de.BirthPlace),
                                    TypeOfBirth = CustomMapper.Mapper.Map<LookupDTO>(de.TypeOfBirth),
                                    EventId = de.EventId,
                                    // Event = CustomMapper.Mapper.Map<EventDTO>(de.Event),
                                    // BirthNotification = CustomMapper.Mapper.Map<BirthNotificationDTO>(de.BirthNotification),
                                    FacilityType = CustomMapper.Mapper.Map<LookupDTO>(de.FacilityType),
                                    Facility = CustomMapper.Mapper.Map<LookupDTO>(de.Facility)
                                }).ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
            // var paymentRateResponse = CustomMapper.Mapper.Map<List<BirthEventDTO>>(paymentRateList);
            // return paymentRateResponse;
        }
    }
}