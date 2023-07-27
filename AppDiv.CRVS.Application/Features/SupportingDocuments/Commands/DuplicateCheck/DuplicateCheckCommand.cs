using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Models;
using MediatR;

namespace AppDiv.CRVS.Application.Features.SupportingDocuments.Commands.DuplicateCheck
{
    public class DuplicateCheckCommand : IRequest<DuplicateCheckCommandResponse>
    {
        public Dictionary<string, BiometricImages> BiometricData { get; set; }
        public Event SavedEvent { get; set; }
    }
    public class DuplicateCheckCommandHandler : IRequestHandler<DuplicateCheckCommand, DuplicateCheckCommandResponse>
    {
        private readonly IRequestApiService _requestApiService;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IPersonDuplicateRepository _personDuplicateRepository;
        private readonly IEventDuplicateRepository _eventDuplicateRepository;
        private readonly IEventRepository _eventRepository;

        public DuplicateCheckCommandHandler(IRequestApiService requestApiService, IPersonalInfoRepository personalInfoRepository, IPersonDuplicateRepository personDuplicateRepository, IEventDuplicateRepository eventDuplicateRepository, IEventRepository eventRepository)
        {
            _requestApiService = requestApiService;
            _personalInfoRepository = personalInfoRepository;
            _personDuplicateRepository = personDuplicateRepository;
            _eventDuplicateRepository = eventDuplicateRepository;
            _eventRepository = eventRepository;
        }
        public async Task<DuplicateCheckCommandResponse> Handle(DuplicateCheckCommand request, CancellationToken cancellationToken)
        {
            // check duplicate person data in the event
            DuplicateCheckCommandResponse duplicateCheckCommandResponse = new DuplicateCheckCommandResponse();

            foreach (var singleBiometric in request.BiometricData)
            {

                IdentifayFingerDto response;
                var req = new FingerPrintApiRequestDto
                {
                    registrationID = null,
                    images = singleBiometric.Value,
                };
                response = await identify(req);
                if (response?.operationResult.ToUpper() == "MATCH_FOUND")
                {
                    var res = await checkDuplicatePerson(new Guid(response.bestResult.id), new Guid(singleBiometric.Key), cancellationToken);
                    duplicateCheckCommandResponse.hasDuplicatePersonalInfo = res;
                }

            }


            IdentifayFingerDto apiResponse;
            // check duplicate event data if event owner has biometric supporting doc
            if (request.BiometricData.ContainsKey(request.SavedEvent.EventOwenerId.ToString()))
            {
                var identifyRequest = new FingerPrintApiRequestDto
                {
                    registrationID = null,
                    images = request.BiometricData[request.SavedEvent.EventOwenerId.ToString()]

                };
                apiResponse = await identify(identifyRequest);

                if (apiResponse?.operationResult.ToUpper() == "MATCH_FOUND")
                {
                    var res = await checkDuplicateEvent(new Guid(apiResponse.bestResult.id), request.SavedEvent, cancellationToken);
                    duplicateCheckCommandResponse.isEventDuplicate = res.isDuplicate;
                    duplicateCheckCommandResponse.DuplicateEventId = res.duplicateEventId;

                }
            }


            duplicateCheckCommandResponse.Message = duplicateCheckCommandResponse.isEventDuplicate
                                                ? "duplicate data found : the event owner has the same registered event in database"
                                                : duplicateCheckCommandResponse.hasDuplicatePersonalInfo
                                                ? "duplicate data found : some peronal informations are duplicated"
                                                : "no duplicate data found";
            return duplicateCheckCommandResponse;

        }





        private async Task<IdentifayFingerDto> identify(FingerPrintApiRequestDto request)
        {
            var resBody = await _requestApiService.post("Identify", request);
            return JsonSerializer.Deserialize<IdentifayFingerDto>(resBody);
        }
        private async Task<bool> checkDuplicatePerson(Guid bestResultId, Guid savedPersonId, CancellationToken cancellationToken)
        {

            var duplicatePerson = _personalInfoRepository.GetAll()
                .Where(e => e.Id == bestResultId && e.Id != savedPersonId)
                .FirstOrDefault();
            if (duplicatePerson == null)
            {
                //TODO:delete biometeric data
                return false;
            }
            var newPersonDuplicate = new PersonDuplicate
            {
                OldPersonId = duplicatePerson.Id,
                NewPersonId = savedPersonId,
                FoundWith = "fingerPrint",
                Status = "unchecked",
            };
            await _personDuplicateRepository.InsertAsync(newPersonDuplicate, cancellationToken);
            await _personDuplicateRepository.SaveChangesAsync(cancellationToken);
            return true;


        }
        private async Task<(bool isDuplicate, Guid? duplicateEventId)> checkDuplicateEvent(Guid bestResultId, Event savedEvent, CancellationToken cancellationToken)
        {
            var duplicateEvent = _eventRepository.GetAll()
                            .Where(e => e.EventOwenerId == bestResultId
                                 && e.EventType.ToLower() == savedEvent.EventType.ToLower()
                                 && e.Id != savedEvent.Id)
                            .FirstOrDefault();
            if (duplicateEvent != null)
            {
                var newEventDuplicate = new EventDuplicate
                {
                    OldEventId = duplicateEvent.Id,
                    NewEventId = savedEvent.Id,
                    FoundWith = "fingerPrint",
                    Status = "unchecked",
                };
                await _eventDuplicateRepository.InsertAsync(newEventDuplicate, cancellationToken);
                await _eventDuplicateRepository.SaveChangesAsync(cancellationToken);
                return (isDuplicate: true, duplicateEventId: duplicateEvent.Id);


            }
            return (isDuplicate: false, duplicateEventId: null);

        }
    }


}