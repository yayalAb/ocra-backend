using System.Text;
using System.Text.Json;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.Fingerprint.commands
{

    public class UpdateFingerprintCommands : IRequest<object>
    {
        public string registrationID
        {
            get; set;
        }
        public BiometricImages? images { get; set; }



        // Customer delete command handler with string response as output
        public class UpdateFingerprintCommandsHandler : IRequestHandler<UpdateFingerprintCommands, object>
        {
            private readonly IRequestApiService _apiRequestService;
            private readonly IPersonalInfoRepository _PersonRepo;
            public UpdateFingerprintCommandsHandler(IRequestApiService apiRequestService, IPersonalInfoRepository PersonRepo)
            {
                _apiRequestService = apiRequestService;
                _PersonRepo = PersonRepo;
            }

            public async Task<object> Handle(UpdateFingerprintCommands request, CancellationToken cancellationToken)
            {
                FingerPrintResponseDto ApiResponse;
                IdentifyFingerDuplicationDto IdentifayedUser;
                try
                {
                    var Create = new FingerPrintApiRequestDto
                    {
                        registrationID = request.registrationID,
                        images = request.images

                    };
                    var responseBody = await _apiRequestService.post("Update", Create);
                    ApiResponse = JsonSerializer.Deserialize<FingerPrintResponseDto>(responseBody);
                    if (ApiResponse.operationResult == "MATCH_FOUND")
                    {
                        Create.registrationID = null;
                        var IdentfaydUser = await _apiRequestService.post("Identify", Create);
                        IdentifayedUser = JsonSerializer.Deserialize<IdentifyFingerDuplicationDto>(IdentfaydUser);
                        var person = _PersonRepo.GetAll()
                        .Include(x => x.PlaceOfBirthLookup)
                        .Include(x => x.NationalityLookup)
                        .Include(x => x.TitleLookup)
                        .Include(x => x.ReligionLookup)
                        .Include(x => x.EducationalStatusLookup)
                        .Include(x => x.TypeOfWorkLookup)
                        .Include(x => x.MarraigeStatusLookup)
                        .Where(x => x.Id == new Guid(IdentifayedUser.bestResult.id)).FirstOrDefault();
                        if (person != null)
                        {
                            var personalInfo = new PersonalInfoDTO
                            {
                                Id = person.Id,
                                FirstName = person?.FirstNameLang,
                                MiddleName = person?.MiddleNameLang,
                                LastName = person?.LastNameLang,
                                BirthDateEt = person?.BirthDateEt,
                                BirthDate = person?.BirthDate,
                                NationalId = person?.NationalId,
                                PlaceOfBirthLookup = person?.PlaceOfBirthLookup?.ValueLang,
                                NationalityLookup = person?.NationalityLookup?.ValueLang,
                                TitleLookup = person?.TitleLookup?.ValueLang,
                                ReligionLookup = person?.ReligionLookup?.ValueLang,
                                EducationalStatusLookup = person?.EducationalStatusLookup?.ValueLang,
                                TypeOfWorkLookup = person?.TypeOfWorkLookup?.ValueLang,
                                MarraigeStatusLookup = person?.MarraigeStatusLookup?.ValueLang,
                                NationLookup = person?.BirthDateEt,
                                CreatedDate = person?.CreatedAt
                            };

                            IdentifayedUser.DuplicatedPerson = personalInfo;
                        }
                        return IdentifayedUser;
                    }
                    return ApiResponse;
                }
                catch (Exception exp)
                {
                    throw (new ApplicationException(exp.Message));
                }
            }
        }
    }
}
