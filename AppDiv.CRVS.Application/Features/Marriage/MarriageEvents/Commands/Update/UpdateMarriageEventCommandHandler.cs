﻿using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Application.Features.Marriage.MarriageEvents.Commands;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update
{

    public class UpdateMarriageEventCommandHandler : IRequestHandler<UpdateMarriageEventCommand, UpdateMarriageEventCommandResponse>
    {
        private readonly IMarriageEventRepository _marriageEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IMarriageApplicationRepository _marriageApplicationRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IDivorceEventRepository _divorceEventRepository;
        private readonly IEventPaymentRequestService _paymentRequestService;
        private readonly IAddressLookupRepository _addressRepository;
        private readonly ISupportingDocumentRepository _supportingDocumentRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IPaymentExamptionRequestRepository _paymentExamptionRequestRepository;
        private readonly IEventRepository _eventRepository;

        public UpdateMarriageEventCommandHandler(IMarriageEventRepository marriageEventRepository,
                                                 IPersonalInfoRepository personalInfoRepository,
                                                 IEventDocumentService eventDocumentService,
                                                 IMarriageApplicationRepository marriageApplicationRepository,
                                                 ILookupRepository lookupRepository,
                                                 IDivorceEventRepository divorceEventRepository,
                                                 IEventPaymentRequestService paymentRequestService,
                                                 IAddressLookupRepository addressRepository,
                                                 ISupportingDocumentRepository supportingDocumentRepository,
                                                 ISettingRepository settingRepository,
                                                 IPaymentExamptionRequestRepository paymentExamptionRequestRepository,
                                                 IEventRepository eventRepository
                                                 )
        {
            _marriageEventRepository = marriageEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _eventDocumentService = eventDocumentService;
            _marriageApplicationRepository = marriageApplicationRepository;
            _lookupRepository = lookupRepository;
            _divorceEventRepository = divorceEventRepository;
            _paymentRequestService = paymentRequestService;
            _addressRepository = addressRepository;
            _supportingDocumentRepository = supportingDocumentRepository;
            _settingRepository = settingRepository;
            _paymentExamptionRequestRepository = paymentExamptionRequestRepository;
            _eventRepository = eventRepository;
        }

        public async Task<UpdateMarriageEventCommandResponse> Handle(UpdateMarriageEventCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _marriageEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using (var transaction = request.IsFromCommand ? null : _marriageEventRepository.Database.BeginTransaction())
                {

                    try

                    {
                        var updateMarriageEventCommandResponse = new UpdateMarriageEventCommandResponse();

                        var validator = new UpdateMarriageEventCommandValidator(_lookupRepository, _marriageApplicationRepository, _personalInfoRepository, _divorceEventRepository, _marriageEventRepository, _paymentExamptionRequestRepository, _settingRepository, _addressRepository, _eventRepository);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);

                        //Check and log validation errors
                        if (validationResult.Errors.Count > 0)
                        {
                            updateMarriageEventCommandResponse.Success = false;
                            updateMarriageEventCommandResponse.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                updateMarriageEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                            updateMarriageEventCommandResponse.Message = updateMarriageEventCommandResponse.ValidationErrors[0];
                            updateMarriageEventCommandResponse.Status = 400;
                        }
                        if (updateMarriageEventCommandResponse.Success)
                        {
                            if (request.ValidateFirst == true)
                            {
                                updateMarriageEventCommandResponse.Created(entity: "Death", message: "Valid Input.");
                                return updateMarriageEventCommandResponse;
                            }

                            var supportingDocs = request.Event.EventSupportingDocuments?.Where(doc => doc.Id == null).ToList();
                            var examptionsupportingDocs = request.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id == null).ToList();
                            var correctionSupportingDocs = request.Event.EventSupportingDocuments?.Where(doc => doc.Id != null).ToList();
                            var correctionExamptionsupportingDocs = request.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id != null).ToList();
                            // request.Event.EventSupportingDocuments = null;
                            // if (request.Event.PaymentExamption != null)
                            // {
                            //     request.Event.PaymentExamption.SupportingDocuments = null;
                            // }
                            var marriageEvent = CustomMapper.Mapper.Map<MarriageEvent>(request);
                            marriageEvent.Event.EventType = "Marriage";
                            // await _marriageEventRepository.EFUpdateAsync(marriageEvent);
                            // await _marriageEventRepository.InsertOrUpdateAsync(marriageEvent, true, cancellationToken);

                            // #region create new supporting docs
                            // var docs = await _eventDocumentService.createSupportingDocumentsAsync(supportingDocs, examptionsupportingDocs, marriageEvent.EventId, marriageEvent.Event.PaymentExamption.Id, cancellationToken);
                            // #endregion create new supporting docs
                            // if (!request.IsFromCommand)
                            // {
                            //     await _marriageEventRepository.SaveChangesAsync(cancellationToken);

                            // }
                            var brideHasDivorce = request.BrideInfo.Id != null
                                               && MarriageValidatorFunctions.brideHasDivorceInLessThanDateLimitInSetting((Guid)request.BrideInfo.Id, marriageEvent.Event.EventDateEt, _settingRepository, _personalInfoRepository);
                            var pregnancyFreeSupportingDocAttached = request.Event.EventSupportingDocuments != null
                                                                    && MarriageValidatorFunctions.hasSupportingDoc(request.Event.EventSupportingDocuments, _lookupRepository, "pregnancy free certificate");
                            var hasUnderAgeApprovalSupportingDoc = request.Event.EventSupportingDocuments != null
                                                                   && MarriageValidatorFunctions.hasSupportingDoc(request.Event.EventSupportingDocuments, _lookupRepository, "underage marriage approval");
                            var brideBelowAgeLimit = !MarriageValidatorFunctions.IsAboveTheAgeLimit(request.BrideInfo.BirthDateEt, request.Event.EventDateEt, true, _settingRepository);
                            var groomBelowAgeLimit = !MarriageValidatorFunctions.IsAboveTheAgeLimit(request.Event.EventOwener.BirthDateEt, request.Event.EventDateEt, false, _settingRepository);
                            if ((brideHasDivorce && pregnancyFreeSupportingDocAttached) || ((brideBelowAgeLimit || groomBelowAgeLimit) && hasUnderAgeApprovalSupportingDoc))
                            {
                                marriageEvent.Event.HasPendingDocumentApproval = true;
                            }
                            var personIds = new PersonIdObj
                            {
                                WifeId = marriageEvent.BrideInfo.Id,
                                HusbandId = marriageEvent.Event.EventOwener.Id,
                                WitnessIds = marriageEvent.Witnesses.Select(w => w.WitnessPersonalInfo.Id).ToList()
                            };
                            marriageEvent.Event.EventSupportingDocuments = null;
                            if (marriageEvent.Event.PaymentExamption != null)
                            {
                                marriageEvent.Event.PaymentExamption.SupportingDocuments = null;
                            }
                            await _marriageEventRepository.EFUpdateAsync(marriageEvent,cancellationToken);
                            if (!request.IsFromCommand)
                            {
                                var docs = await _eventDocumentService.createSupportingDocumentsAsync(supportingDocs, examptionsupportingDocs, marriageEvent.EventId, marriageEvent.Event.PaymentExamption?.Id, cancellationToken);
                                var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, docs.supportingDocs);
                                _eventDocumentService.savePhotos(separatedDocs.userPhotos);
                                _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, (ICollection<SupportingDocument>)docs.examptionDocs, "Marriage");
                                _eventDocumentService.saveFingerPrints(separatedDocs.fingerPrint);

                            }
                            else
                            {
                                // await _marriageEventRepository.EFUpdateAsync(marriageEvent);
                                var docs = await _eventDocumentService.createSupportingDocumentsAsync(correctionSupportingDocs, correctionExamptionsupportingDocs, marriageEvent.EventId, marriageEvent.Event.PaymentExamption?.Id, cancellationToken);
                                var separatedDocs = _eventDocumentService.ExtractOldSupportingDocs(personIds, docs.supportingDocs);
                                       if(separatedDocs.userPhotos!=null &&(separatedDocs.userPhotos.Count != 0)){
                                        _eventDocumentService.MovePhotos(separatedDocs.userPhotos, "Marriage");
                                          }

                                _eventDocumentService.MoveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, (ICollection<SupportingDocument>)docs.examptionDocs, "Marriage");
                                //TODO:save fingerprint
                            }
                            // var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, docs.supportingDocs);
                            // _eventDocumentService.savePhotos(separatedDocs.userPhotos);
                            // _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, (ICollection<SupportingDocument>?)docs.examptionDocs, "Marriage");
                            updateMarriageEventCommandResponse.Message = "Marriage Event Updated Successfully";
                            await transaction?.CommitAsync()!;
                        }
                        return updateMarriageEventCommandResponse;

                    }
                    catch (Exception)
                    {
                        await transaction?.RollbackAsync()!;
                        throw;
                    }
                }
            });


        }
    }
}
