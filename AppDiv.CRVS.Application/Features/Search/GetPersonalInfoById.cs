using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.Search
{
    // Customer GetPersonalInfoById with  response
    public class GetPersonalInfoById : IRequest<object>
    {
        public Guid Id { get; set; }

    }

    public class GetPersonalInfoByIdHandler : IRequestHandler<GetPersonalInfoById, object>
    {
        private readonly IPersonalInfoRepository _PersonaInfoRepository;
        private readonly IDateAndAddressService _AddressService;

        public GetPersonalInfoByIdHandler(IPersonalInfoRepository PersonaInfoRepository, IDateAndAddressService AddressService)
        {
            _PersonaInfoRepository = PersonaInfoRepository;
            _AddressService = AddressService;
        }
        public async Task<object> Handle(GetPersonalInfoById request, CancellationToken cancellationToken)
        {
            var SelectedPerson = _PersonaInfoRepository.GetAllQueryable()
            .Include(p => p.Events.Where(e => e.EventType.ToLower() == "birth"))
            .ThenInclude(e => e.BirthEvent)
            .Where(model => model.Id == request.Id)
            .Select(an => new PersonalInfoByIdDTO
            {
                Id = an.Id,
                BirhtCertificateId = an.Events.Where(e => e.EventType.ToLower() == "birth").FirstOrDefault() == null
                                        ? null
                                        : an.Events.Where(e => e.EventType.ToLower() == "birth").FirstOrDefault()!.CertificateId,
                FirstName = an.FirstName,
                MiddleName = an.MiddleName,
                LastName = an.LastName,
                BirthDateEt = an.BirthDateEt,
                NationalId = an.NationalId,
                SexLookupId = an.SexLookupId,
                PlaceOfBirthLookupId = an.PlaceOfBirthLookupId,
                NationalityLookupId = an.NationalityLookupId,
                TitleLookupId = an.TitleLookupId,
                ReligionLookupId = an.ReligionLookupId,
                EducationalStatusLookupId = an.EducationalStatusLookupId,
                TypeOfWorkLookupId = an.TitleLookupId,
                MarriageStatusLookupId = an.MarriageStatusLookupId,
                BirthAddressId = an.BirthAddressId,
                ResidentAddressId = an.ResidentAddressId,
                NationLookupId = an.NationLookupId,
                ContactInfoId = an.ContactInfoId,
            }).FirstOrDefault();
            SelectedPerson.BirthAddressResponseDTO = await _AddressService?.FormatedAddress(SelectedPerson?.BirthAddressId);
            SelectedPerson.ResidentAddressResponseDTO = await _AddressService?.FormatedAddress(SelectedPerson?.ResidentAddressId);

            return SelectedPerson;
        }
    }
}