using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Customers.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetBirthEventByIdQuery : IRequest<BirthEventDTO>
    {
        public Guid Id { get; private set; }

        public GetBirthEventByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetBirthEventByIdHandler : IRequestHandler<GetBirthEventByIdQuery, BirthEventDTO>
    {
        private readonly IBirthEventRepository _BirthEventRepository;
        private readonly IDateAndAddressService _AddressService;
        private readonly IEventDocumentService _eventDocumentService;

        public GetBirthEventByIdHandler(IBirthEventRepository BirthEventRepository, IDateAndAddressService AddressService, IEventDocumentService eventDocumentService)
        {
            _BirthEventRepository = BirthEventRepository;
            _AddressService = AddressService;
            _eventDocumentService = eventDocumentService;
        }
        public async Task<BirthEventDTO> Handle(GetBirthEventByIdQuery request, CancellationToken cancellationToken)
        {

            var selectedBirthEvent = await _BirthEventRepository.GetWithIncludedAsync(request.Id)
                                        ?? throw new NotFoundException("Birth Event with the Given Id Is Not Found");

            var BirthEvent = CustomMapper.Mapper.Map<BirthEventDTO>(selectedBirthEvent);
            BirthEvent.Event.EventSupportingDocuments = (BirthEvent?.Event?.EventSupportingDocuments?.Count == 0 ? null : BirthEvent?.Event?.EventSupportingDocuments)!;
            if (BirthEvent?.Event.PaymentExamption != null){
                BirthEvent.Event.PaymentExamption.SupportingDocuments = (BirthEvent?.Event?.PaymentExamption?.SupportingDocuments?.Count == 0 ? null : BirthEvent?.Event?.PaymentExamption?.SupportingDocuments)!;
            }
            if(BirthEvent!.Father!=null){
            BirthEvent!.Father!.BirthAddressResponseDTO = await _AddressService.FormatedAddress(BirthEvent?.Father?.BirthAddressId)!;
            BirthEvent!.Father!.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(BirthEvent?.Father?.ResidentAddressId)!;
            }
            if(BirthEvent!.Mother!=null){
            BirthEvent!.Mother!.BirthAddressResponseDTO = await _AddressService.FormatedAddress(BirthEvent?.Mother?.BirthAddressId)!;
            BirthEvent!.Mother!.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(BirthEvent?.Mother?.ResidentAddressId)!;
            }

            BirthEvent!.Event!.EventAddressResponseDTO = await _AddressService.FormatedAddress(BirthEvent?.Event?.EventAddressId)!;
            BirthEvent!.Event!.EventOwener.BirthAddressResponseDTO = await _AddressService.FormatedAddress(BirthEvent?.Event.EventOwener?.BirthAddressId)!;
            if (BirthEvent?.Event.EventOwener?.ResidentAddressId != null)
            {
                BirthEvent.Event.EventOwener.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(BirthEvent?.Event.EventOwener?.ResidentAddressId)!;
            }
            if (BirthEvent?.Event.EventRegistrar != null)
            {
                BirthEvent!.Event.EventRegistrar.RegistrarInfo.BirthAddressResponseDTO = await _AddressService.FormatedAddress(BirthEvent?.Event?.EventRegistrar?.RegistrarInfo?.BirthAddressId)!;
                BirthEvent!.Event.EventRegistrar.RegistrarInfo.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(BirthEvent?.Event?.EventRegistrar?.RegistrarInfo?.ResidentAddressId)!;
            }
            BirthEvent.Event.fingerPrints = new
            {
                Mother = _eventDocumentService.getSingleFingerprintUrls(BirthEvent.Mother?.Id.ToString()),
                Father = _eventDocumentService.getSingleFingerprintUrls(BirthEvent.Father?.Id.ToString()),
                Child = _eventDocumentService.getSingleFingerprintUrls(BirthEvent.Event.EventOwener?.Id.ToString()),
                Registrar = _eventDocumentService.getSingleFingerprintUrls(BirthEvent.Event.EventRegistrar?.RegistrarInfo?.Id.ToString())
            };
            return BirthEvent!;
        }
    }
}