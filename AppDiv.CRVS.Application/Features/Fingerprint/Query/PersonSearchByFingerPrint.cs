
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Fingerprint.Query
{
    public class PersonSearchByFingerPrint : IRequest<List<PersonSearchResponse>>
    {
        public BiometricImages Base64Image { get; set; }
    }

    public class PersonSearchByFingerPrintHandler : IRequestHandler<PersonSearchByFingerPrint, List<PersonSearchResponse>>
    {

        private readonly IEventRepository _eventRepository;
        private readonly IRequestApiService _apiRequestService;
        private readonly IUserResolverService _userResolverService;
        private readonly IPersonalInfoRepository _personalInfoRepo;

        public PersonSearchByFingerPrintHandler(IPersonalInfoRepository personalInfoRepo,IEventRepository eventQueryRepository, IRequestApiService apiRequestService, IUserResolverService userResolverService)
        {
            _eventRepository = eventQueryRepository;
            _apiRequestService = apiRequestService;
            _userResolverService = userResolverService;
            _personalInfoRepo=personalInfoRepo;
        }
        public async Task<List<PersonSearchResponse>> Handle(PersonSearchByFingerPrint request, CancellationToken cancellationToken)
        {
            var PersonalInfo = new List<PersonSearchResponse>();
            IdentifayFingerDto ApiResponse;
            try
            {
                var Create = new FingerPrintApiRequestDto
                {
                    registrationID = null,
                    images = request.Base64Image

                };
                var responseBody = await _apiRequestService.post("Identify", Create);
                ApiResponse = JsonSerializer.Deserialize<IdentifayFingerDto>(responseBody);
                if (ApiResponse.operationResult == "MATCH_FOUND")
                {
                    PersonalInfo = _personalInfoRepo.GetAll()
                    .Include(a=>a.ResidentAddress)
                    .Where(x => x.Id == new Guid(ApiResponse.bestResult.id)).Select(x=>new PersonSearchResponse {
                     Id =x.Id,
                     FullName =x.FirstNameLang+" " +x.MiddleNameLang+" "+x.LastNameLang,
                     Address =x.ResidentAddress.AddressNameLang,
                     NationalId =x.NationalId
                    }).ToList();
                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return PersonalInfo;
        }
    }
}