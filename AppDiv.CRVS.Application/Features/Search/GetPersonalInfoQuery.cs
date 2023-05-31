using System;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.Search
{
    // Customer GetPersonalInfoQuery with  response
    public class GetPersonalInfoQuery : IRequest<object>
    {
        public string SearchString { get; set; }
        public string? gender { get; set; }
        public int age { get; set; }

    }

    public class GetPersonalInfoQueryHandler : IRequestHandler<GetPersonalInfoQuery, object>
    {
        private readonly IPersonalInfoRepository _PersonaInfoRepository;

        public GetPersonalInfoQueryHandler(IPersonalInfoRepository PersonaInfoRepository)
        {
            _PersonaInfoRepository = PersonaInfoRepository;
        }
        public async Task<object> Handle(GetPersonalInfoQuery request, CancellationToken cancellationToken)
        {
            var SelectedInfo = _PersonaInfoRepository.GetAll();

            if (request.gender != null && request.age > 0)
            {
                SelectedInfo = SelectedInfo.Where(model => EF.Functions.Like(model.SexLookup.ValueStr, $"%{request.gender}%")
                 && (model.BirthDate.HasValue ? model.BirthDate.Value : default(DateTime)) <= DateTime.Now.AddYears(-request.age));
            }
            else if (request.age > 0 && request.gender == null)
            {
                SelectedInfo = SelectedInfo.Where(model => (model.BirthDate.HasValue ? model.BirthDate.Value : default(DateTime)) <= DateTime.Now.AddYears(-request.age));
            }
            var response = SelectedInfo.Where(model =>
                                     EF.Functions.Like(model.FirstNameStr, $"%{request.SearchString}%")
                                 || EF.Functions.Like(model.MiddleNameStr, $"%{request.SearchString}%")
                                 || EF.Functions.Like(model.LastNameStr, $"%{request.SearchString}%")
                                 || EF.Functions.Like(model.NationalId, $"%{request.SearchString}%")
                                 || EF.Functions.Like(model.SexLookup.ValueStr, $"%{request.SearchString}%")
                                 || EF.Functions.Like(model.TypeOfWorkLookup.ValueStr, $"%{request.SearchString}%")
                                 || EF.Functions.Like(model.TitleLookup.ValueStr, $"%{request.SearchString}%")
                                 || EF.Functions.Like(model.MarraigeStatusLookup.ValueStr, $"%{request.SearchString}%"))
                                 .Select(an => new PersonalInfoSearchDTO
                                 {
                                     Id = an.Id,
                                     FirstName = an.FirstNameLang,
                                     MiddleName = an.MiddleNameLang,
                                     LastName = an.LastNameLang,
                                     NationalId = an.NationalId,
                                     BirthDateEt = an.BirthDateEt,
                                     Gender = string.IsNullOrEmpty(an.SexLookup.ValueLang) ? null : an.SexLookup.ValueLang,
                                     Address = string.IsNullOrEmpty(an.BirthAddress.AddressNameLang) ? null : an.BirthAddress.AddressNameLang,
                                     NationalityLookup = string.IsNullOrEmpty(an.NationalityLookup.ValueLang) ? null : an.NationalityLookup.ValueLang,
                                     TitleLookup = string.IsNullOrEmpty(an.TitleLookup.ValueLang) ? null : an.TitleLookup.ValueLang,
                                     TypeOfWorkLookup = string.IsNullOrEmpty(an.TypeOfWorkLookup.ValueLang) ? null : an.TypeOfWorkLookup.ValueLang
                                 }).Take(50);

            return response;

        }
    }
}