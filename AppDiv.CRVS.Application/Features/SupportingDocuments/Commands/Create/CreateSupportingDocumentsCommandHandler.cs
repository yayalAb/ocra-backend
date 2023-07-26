using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace AppDiv.CRVS.Application.Features.SupportingDocuments.Commands.Create
{
    public class CreateSupportingDocumentsCommandHandler : IRequestHandler<CreateSupportingDocumentsCommand, CreateSupportingDocumentsCommandResponse>
    {
        private readonly ISupportingDocumentRepository _supportingDocumentRepo;
        private readonly IEventRepository _eventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IPersonDuplicateRepository _personDuplicateRepository;
        private readonly IEventDuplicateRepository _eventDuplicateRepository;
        private readonly IRequestApiService _requestApiService;

        public CreateSupportingDocumentsCommandHandler(ISupportingDocumentRepository supportingDocumentRepository,
                                                       IEventRepository eventRepository,
                                                       IEventDocumentService eventDocumentService,
                                                       IPersonalInfoRepository personalInfoRepository,
                                                       IPersonDuplicateRepository personDuplicateRepository,
                                                       IEventDuplicateRepository eventDuplicateRepository,
                                                       IRequestApiService requestApiService)
        {
            _supportingDocumentRepo = supportingDocumentRepository;
            _eventRepository = eventRepository;
            _eventDocumentService = eventDocumentService;
            _personalInfoRepository = personalInfoRepository;
            _personDuplicateRepository = personDuplicateRepository;
            _eventDuplicateRepository = eventDuplicateRepository;
            _requestApiService = requestApiService;
        }
        public async Task<CreateSupportingDocumentsCommandResponse> Handle(CreateSupportingDocumentsCommand request, CancellationToken cancellationToken)
        {
            var CreateSupportingDocumentsCommandResponse = new CreateSupportingDocumentsCommandResponse(_supportingDocumentRepo);
            var validator = new CreateSupportingDocumentsCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.Errors.Count > 0)
            {
                CreateSupportingDocumentsCommandResponse.Success = false;
                CreateSupportingDocumentsCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateSupportingDocumentsCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateSupportingDocumentsCommandResponse.Message = CreateSupportingDocumentsCommandResponse.ValidationErrors[0];
            }
            if (CreateSupportingDocumentsCommandResponse.Success)
            {
                var savedEvent = await _eventRepository.GetAll()
                                    .Where(e => e.Id == request.EventId)
                                    .Include(e => e.MarriageEvent).ThenInclude(m => m.Witnesses)
                                    .Include(e => e.BirthEvent)
                                    .Include(e => e.AdoptionEvent)
                                    .Include(e => e.DivorceEvent)
                                    .Include(e => e.DeathEventNavigation).FirstOrDefaultAsync();
                if (savedEvent == null)
                {
                    throw new NotFoundException($"Could not Save supporting Doucments : \n event with Id {request.EventId} is not found");
                }

                var biometricData = await _eventDocumentService.SaveSupportingDocumentsAsync(savedEvent, request.EventSupportingDocuments, request.ExamptionSupportingDocuments, request.PaymentExamptionId, cancellationToken);

                foreach (var f in biometricData.fingerPrints.Where(f => f.Key != savedEvent.EventOwenerId.ToString()))
                {

                    IdentifayFingerDto response;

                    var req = new FingerPrintApiRequestDto
                    {
                        registrationID = null,
                        images = biometricData.fingerPrints[savedEvent.EventOwenerId.ToString()]

                    };
                    var resBody = await _requestApiService.post("Identify", req);
                    response = JsonSerializer.Deserialize<IdentifayFingerDto>(resBody);
                    if (response?.operationResult.ToUpper() == "MATCH_FOUND")
                    {
                        var duplicatePerson = _personalInfoRepository.GetAll()
                                        .Where(e => e.Id == new Guid(response.bestResult.id))
                                        .FirstOrDefault();
                        if (duplicatePerson == null)
                        {
                            //TODO:delete biometeric data
                        }
                        else
                        {
                            var newPersonDuplicate = new PersonDuplicate
                            {
                                OldPersonId = duplicatePerson.Id,
                                NewPersonId = new Guid(response.bestResult.id),
                                FoundWith = "fingerPrint",
                                Status = "unchecked",
                            };
                            await _personDuplicateRepository.InsertAsync(newPersonDuplicate, cancellationToken);
                            await _personDuplicateRepository.SaveChangesAsync(cancellationToken);
                        }

                    }

                }

                IdentifayFingerDto apiResponse;
                var res = new IdentifayFingerDto();

                var identifyRequest = new FingerPrintApiRequestDto
                {
                    registrationID = null,
                    images = biometricData.fingerPrints[savedEvent.EventOwenerId.ToString()]

                };
                var responseBody = await _requestApiService.post("Identify", identifyRequest);
                apiResponse = JsonSerializer.Deserialize<IdentifayFingerDto>(responseBody);

                if (apiResponse?.operationResult.ToUpper() == "MATCH_FOUND")
                {
                    var duplicateEvent = _eventRepository.GetAll()
                               .Where(e => e.EventOwenerId == new Guid(res.bestResult.id)
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


                    }
                    else
                    {

                        var duplicatePerson = _personalInfoRepository.GetAll()
                                        .Where(e => e.Id == new Guid(res.bestResult.id))
                                        .FirstOrDefault();
                        if (duplicatePerson == null)
                        {
                            //TODO:delete biometeric data
                        }
                        else
                        {
                            var newPersonDuplicate = new PersonDuplicate
                            {
                                OldPersonId = duplicatePerson.Id,
                                NewPersonId = new Guid(res.bestResult.id),
                                FoundWith = "fingerPrint",
                                Status = "unchecked",
                            };
                            await _personDuplicateRepository.InsertAsync(newPersonDuplicate, cancellationToken);
                            await _personDuplicateRepository.SaveChangesAsync(cancellationToken);
                        }

                    }

                }
                else
                {
                    CreateSupportingDocumentsCommandResponse.Message = "Supporting document created successfully!";

                }
            }
            return CreateSupportingDocumentsCommandResponse;
        }
    }
}