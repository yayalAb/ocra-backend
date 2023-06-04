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
    // Customer getPersonForPrivewQuery with  response
    public class getPersonForPrivewQuery : IRequest<GetPersonByIdString>
    {
        public Guid Id { get; set; }

    }

    public class getPersonForPrivewQueryHandler : IRequestHandler<getPersonForPrivewQuery, GetPersonByIdString>
    {
        private readonly IPersonalInfoRepository _PersonaInfoRepository;

        private readonly IDateAndAddressService _DateAndAddressService;

        public getPersonForPrivewQueryHandler(IPersonalInfoRepository PersonaInfoRepository, IDateAndAddressService DateAndAddressService)
        {
            _PersonaInfoRepository = PersonaInfoRepository;
            _DateAndAddressService = DateAndAddressService;
        }
        public async Task<GetPersonByIdString> Handle(getPersonForPrivewQuery request, CancellationToken cancellationToken)
        {
            var personForAddress = await _PersonaInfoRepository.GetAsync(request.Id);


            var Birthaddress = (personForAddress?.BirthAddressId == Guid.Empty
               || personForAddress?.BirthAddressId == null) ? null :
               _DateAndAddressService.SplitedAddressByLang(personForAddress.BirthAddressId);

            var Residentaddress = (personForAddress?.BirthAddressId == Guid.Empty
              || personForAddress?.BirthAddressId == null) ? null :
              _DateAndAddressService.SplitedAddressByLang(personForAddress.BirthAddressId);


            var SelectedPerson = _PersonaInfoRepository.GetAll().Where(model => model.Id == request.Id)
            .Select(an => new GetPersonByIdString
            {
                Id = an.Id,
                FirstName = an.FirstNameLang,
                MiddleName = an.MiddleNameLang,
                LastName = an.LastNameLang,
                BirthDateEt = an.BirthDateEt,
                NationalId = an.NationalId,
                PhoneNo = an.PhoneNumber,
                Sex = an.SexLookup.ValueLang,
                PlaceOfBirth = an.PlaceOfBirthLookup.ValueLang,
                Nationality = an.NationalityLookup.ValueLang,
                Title = an.TitleLookup.ValueLang,
                Religion = an.ReligionLookup.ValueLang,
                EducationalStatus = an.EducationalStatusLookup.ValueLang,
                TypeOfWork = an.TitleLookup.ValueLang,
                MarriageStatus = an.MarraigeStatusLookup.ValueLang,
                Nation = an.NationalityLookup.ValueLang,
                BirthAddressCountry = Birthaddress.ElementAtOrDefault(0),
                BirthAddressRegion = Birthaddress.ElementAtOrDefault(1),
                BirthAddressZone = Birthaddress.ElementAtOrDefault(2),
                BirthAddressWoreda = Birthaddress.ElementAtOrDefault(3),
                BirthAddressKebele = Birthaddress.ElementAtOrDefault(4),

                ResidentAddressCountry = Residentaddress.ElementAtOrDefault(0),
                ResidentAddressRegion = Residentaddress.ElementAtOrDefault(1),
                ResidentAddressZone = Residentaddress.ElementAtOrDefault(2),
                ResidentAddressWoreda = Residentaddress.ElementAtOrDefault(3),
                ResidentAddressKebele = Residentaddress.ElementAtOrDefault(4),
            }).FirstOrDefault();
            return SelectedPerson;
        }
    }
}