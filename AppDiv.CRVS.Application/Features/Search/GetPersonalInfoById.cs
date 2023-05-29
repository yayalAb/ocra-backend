using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Mapper;

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

        public GetPersonalInfoByIdHandler(IPersonalInfoRepository PersonaInfoRepository)
        {
            _PersonaInfoRepository = PersonaInfoRepository;
        }
        public async Task<object> Handle(GetPersonalInfoById request, CancellationToken cancellationToken)
        {
            var SelectedPerson = _PersonaInfoRepository.GetAll().Where(model => model.Id == request.Id)
            .Select(an => new PersonalInfoByIdDTO
            {
                Id = an.Id,
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
                NationLookupId = an.NationalityLookupId,
                ContactInfoId = an.ContactInfoId,
            });

            return SelectedPerson;
        }
    }
}