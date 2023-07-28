using AppDiv.CRVS.Application.Contracts.DTOs;
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
    public class SearchByFingerprint : IRequest<List<SearchCertificateResponseDTO>>
    {
        public BiometricImages Base64Image { get; set; }
    }

    public class SearchByFingerprintHandler : IRequestHandler<SearchByFingerprint, List<SearchCertificateResponseDTO>>
    {

        private readonly IEventRepository _eventRepository;
        private readonly IRequestApiService _apiRequestService;
        private readonly IUserResolverService _userResolverService;

        public SearchByFingerprintHandler(IEventRepository eventQueryRepository, IRequestApiService apiRequestService, IUserResolverService userResolverService)
        {
            _eventRepository = eventQueryRepository;
            _apiRequestService = apiRequestService;
            _userResolverService = userResolverService;
        }
        public async Task<List<SearchCertificateResponseDTO>> Handle(SearchByFingerprint request, CancellationToken cancellationToken)
        {
            var listOfCertifcates = new List<SearchCertificateResponseDTO>();
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
                    Console.WriteLine("Event Found ! {0}", ApiResponse.bestResult.id);
                    var userEvents = _eventRepository.GetAll()
                    .Include(x => x.BirthEvent)
                    .Include(x => x.DivorceEvent)
                    .Include(x => x.DeathEventNavigation)
                    .Include(x => x.AdoptionEvent)
                    .Include(x => x.MarriageEvent)
                    .Include(x => x.EventOwener)
                    .Include(x => x.CivilRegOfficer)
                    .Include(x => x.EventAddress)
                    .Where(x => x.EventOwenerId == new Guid(ApiResponse.bestResult.id));
                    if (userEvents.FirstOrDefault() != null)
                    {
                        listOfCertifcates = userEvents.Select(d => new SearchCertificateResponseDTO
                        {
                            Id = d.EventOwenerId,
                            EventId = d.Id,
                            NestedEventId = d.BirthEvent == null ? d.DivorceEvent == null ? d.DeathEventNavigation == null ? d.AdoptionEvent == null ? d.MarriageEvent == null ? Guid.Empty : d.MarriageEvent.Id : d.AdoptionEvent.Id : d.DeathEventNavigation.Id : d.DivorceEvent.Id : d.BirthEvent.Id,
                            FullName = d.EventOwener.FirstNameLang + " " + d.EventOwener.MiddleNameLang + " " + d.EventOwener.LastNameLang,
                            // MotherName = d.EventOwener.MiddleNameLang,
                            CivilRegOfficerName = d.CivilRegOfficer.FirstNameLang + " " + d.CivilRegOfficer.MiddleNameLang + " " + d.CivilRegOfficer.LastNameLang,
                            Address = d.EventAddress.AddressNameLang,
                            EventAddress = d.EventAddress.AddressNameLang,
                            NationalId = d.EventOwener.NationalId,
                            CertificateId = d.CertificateId,
                            EventType = d.EventType,
                            CertificateSerialNumber = d.CertificateId,
                            CanViewDetail = d.EventRegisteredAddressId == _userResolverService.GetWorkingAddressId()
                        })
                           .ToList();
                        Console.WriteLine("Event Found !");
                    }
                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return listOfCertifcates;
        }
    }
}