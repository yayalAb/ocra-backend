using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Certificates.Command.Create;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Application.Service.ArchiveService;
using AppDiv.CRVS.Application.Features.CertificateTemplatesLookup.Query.GetCertificateTemplates;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Update;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.Archives.Query
{

    // Customer GenerateCustomerQuery with Customer response
    public class GenerateArchivePreviewQuery : IRequest<object>
    {
        public JObject Content { get; set; }
        public string EventType { get; set; }
        public string Command { get; set; }

    }

    public class GenerateArchivePreviewHandler : IRequestHandler<GenerateArchivePreviewQuery, object>
    {
        private readonly IArchiveGenerator _archiveGenerator;

        private readonly IMediator _mediator;

        public GenerateArchivePreviewHandler(IMediator mediator,
                                        IArchiveGenerator archiveGenerator)
        {
            this._mediator = mediator;
            _archiveGenerator = archiveGenerator;
        }
        public async Task<object> Handle(GenerateArchivePreviewQuery request, CancellationToken cancellationToken)
        {
            var preview = new ArchiveResponseDTO();
            switch (request.EventType)
            {
                case "Birth":
                    preview.Content = request.Command switch
                    {
                        "Create" => 
                        _archiveGenerator.GetBirthArchivePreview(
                            CustomMapper.Mapper.Map<BirthEvent>(
                                ReturnArchiveFromJObject.GetArchive<AddBirthEventRequest>(request.Content)),
                            ""),
                        "Update" => _archiveGenerator.GetBirthArchivePreview(
                            CustomMapper.Mapper.Map<BirthEvent>(
                                ReturnArchiveFromJObject.GetArchive<UpdateBirthEventCommand>(request.Content)),
                            ""),
                    };
                    break;
                case "Death":
                    preview.Content = request.Command switch
                    {
                        "Create" => 
                        _archiveGenerator.GetDeathArchivePreview(
                            CustomMapper.Mapper.Map<DeathEvent>(
                                ReturnArchiveFromJObject.GetArchive<AddDeathEventRequest>(request.Content)),
                            ""),
                        "Update" => _archiveGenerator.GetDeathArchivePreview(
                            CustomMapper.Mapper.Map<DeathEvent>(
                                ReturnArchiveFromJObject.GetArchive<UpdateDeathEventCommand>(request.Content)),
                            ""),
                    };
                    break;
                case "Adoption":
                    preview.Content = request.Command switch
                    {
                        "Create" => 
                        _archiveGenerator.GetAdoptionArchivePreview(
                            CustomMapper.Mapper.Map<AdoptionEvent>(
                                ReturnArchiveFromJObject.GetArchive<AddAdoptionRequest>(request.Content)),
                            ""),
                        "Update" => _archiveGenerator.GetAdoptionArchivePreview(
                            CustomMapper.Mapper.Map<AdoptionEvent>(
                                ReturnArchiveFromJObject.GetArchive<UpdateAdoptionCommand>(request.Content)),
                            ""),
                    };
                    break;
                case "Divorce":
                    preview.Content = request.Command switch
                    {
                        "Create" => 
                        _archiveGenerator.GetDivorceArchivePreview(
                            CustomMapper.Mapper.Map<DivorceEvent>(
                                ReturnArchiveFromJObject.GetArchive<CreateDivorceEventCommand>(request.Content)),
                            ""),
                        "Update" => _archiveGenerator.GetDivorceArchivePreview(
                            CustomMapper.Mapper.Map<DivorceEvent>(
                                ReturnArchiveFromJObject.GetArchive<UpdateDivorceEventCommand>(request.Content)),
                            ""),
                    };
                    break;
                case "Marriage":
                    preview.Content = request.Command switch
                    {
                        "Create" => 
                        _archiveGenerator.GetMarriageArchivePreview(
                            CustomMapper.Mapper.Map<MarriageEvent>(
                                ReturnArchiveFromJObject.GetArchive<CreateMarriageEventCommand>(request.Content)),
                            ""),
                        "Update" => _archiveGenerator.GetMarriageArchivePreview(
                            CustomMapper.Mapper.Map<MarriageEvent>(
                                ReturnArchiveFromJObject.GetArchive<UpdateMarriageEventCommand>(request.Content)),
                            ""),
                    };
                    break;
                default:
                    return new BaseResponse("Unknown Event type", false, 400);

            }
            preview.TemplateId = await _mediator.Send(new GetCertificateTemplatesByNameQuery { Name = request.EventType  + " " + "Archive" });
            return preview;
        }

    }
}