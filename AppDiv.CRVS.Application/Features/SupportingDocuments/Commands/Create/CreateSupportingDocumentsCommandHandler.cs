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
using AppDiv.CRVS.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace AppDiv.CRVS.Application.Features.SupportingDocuments.Commands.Create
{
    public class CreateSupportingDocumentsCommandHandler : IRequestHandler<CreateSupportingDocumentsCommand, CreateSupportingDocumentsCommandResponse>
    {
        private readonly ISupportingDocumentRepository _supportingDocumentRepo;
        private readonly IEventRepository _eventRepository;
        private readonly IEventDocumentService _eventDocumentService;


        public CreateSupportingDocumentsCommandHandler(ISupportingDocumentRepository supportingDocumentRepository,
                                                       IEventRepository eventRepository,
                                                       IEventDocumentService eventDocumentService
                                          )
        {
            _supportingDocumentRepo = supportingDocumentRepository;
            _eventRepository = eventRepository;
            _eventDocumentService = eventDocumentService;
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
                    CreateSupportingDocumentsCommandResponse.Success = false;
                    CreateSupportingDocumentsCommandResponse.Message = $"Could not save supporting Doucments : \n event with Id {request.EventId} is not found";
                    return CreateSupportingDocumentsCommandResponse;
                }

                var response = await _eventDocumentService.SaveSupportingDocumentsAsync(savedEvent, request.EventSupportingDocuments, request.ExamptionSupportingDocuments, request.PaymentExamptionId, cancellationToken);
                var biometricData = mergeBiometricData(response);

                CreateSupportingDocumentsCommandResponse.Message = "Supporting document created successfully!";
                CreateSupportingDocumentsCommandResponse.BiometricData = biometricData;
                CreateSupportingDocumentsCommandResponse.SavedEvent = savedEvent;
            }
            return CreateSupportingDocumentsCommandResponse;
        }
        private Dictionary<string, BiometricImages> mergeBiometricData((Dictionary<string, string> userPhotos, Dictionary<string, List<BiometricImagesAtt>> fingerPrints) supportingDocs)
        {
            var fingerPrints = supportingDocs.fingerPrints.ToList();
            Dictionary<string, BiometricImages> BiometricInfo = new Dictionary<string, BiometricImages>();
            var userPhotos = supportingDocs.userPhotos.ToList();
            fingerPrints.ForEach(f =>
            {
                BiometricInfo.Add(f.Key, new BiometricImages
                {
                    fingerprint = f.Value
                });
            });
            userPhotos.ForEach(p =>
            {
                if (BiometricInfo.ContainsKey(p.Key))
                {
                    // BiometricInfo[p.Key].face = new List<BiometricImagesAtt>{
                    //    new BiometricImagesAtt{
                    //     position = 0,
                    //     base64Image = p.Value
                    //    }
                    // };
                }
                else
                {
                    BiometricInfo.Add(p.Key, new BiometricImages
                    {
                        // face = new List<BiometricImagesAtt>{
                        //     new BiometricImagesAtt{
                        //         position = 0 ,
                        //         base64Image = p.Value
                        //     }
                        // }
                    });
                }
            });
            return BiometricInfo;
        }
    }
}