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


            (string am, string or)? Birthaddress = (personForAddress?.BirthAddressId == Guid.Empty
               || personForAddress?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(personForAddress.BirthAddressId);

            (string am, string or)? Residentaddress = (personForAddress?.BirthAddressId == Guid.Empty
              || personForAddress?.BirthAddressId == null) ? null :
              _DateAndAddressService.addressFormat(personForAddress.BirthAddressId);


            var SelectedPerson = _PersonaInfoRepository.GetAll().Where(model => model.Id == request.Id)
            .Select(an => new GetPersonByIdString
            {
                Id = an.Id,
                FirstName = an.FirstNameLang,
                MiddleName = an.MiddleNameLang,
                LastName = an.LastNameLang,
                BirthDateEt = an.BirthDateEt,
                National = an.NationalId,
                Sex = an.SexLookup.ValueLang,
                PlaceOfBirth = an.PlaceOfBirthLookup.ValueLang,
                Nationality = an.NationalityLookup.ValueLang,
                Title = an.TitleLookup.ValueLang,
                Religion = an.ReligionLookup.ValueLang,
                EducationalStatus = an.EducationalStatusLookup.ValueLang,
                TypeOfWork = an.TitleLookup.ValueLang,
                MarriageStatus = an.MarraigeStatusLookup.ValueLang,
                Nation = an.NationalityLookup.ValueLang,
                BirthAddressOr = Birthaddress.Value.or,
                BirthAddressAm = Birthaddress.Value.am,
                ResidentAddressOr = Residentaddress.Value.or,
                ResidentAddressAm = Residentaddress.Value.am,

            }).FirstOrDefault();
            return SelectedPerson;
        }
    }
}