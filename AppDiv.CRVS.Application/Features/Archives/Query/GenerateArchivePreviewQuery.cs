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

namespace AppDiv.CRVS.Application.Features.Archives.Query
{

    // Customer GenerateCustomerQuery with Customer response
    public class GenerateArchivePreviewQuery : IRequest<object>
    {
        public JObject Content { get; set; }
        public string EventType { get; set; }

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
            return new ArchiveResponseDTO 
            {
                Content = request.EventType switch
                            {
                                "Birth" => _archiveGenerator.GetBirthArchivePreview(ReturnArchiveFromJObject.GetArchive<BirthEvent>(request.Content), ""),
                                "Death" => _archiveGenerator.GetDeathArchivePreview(ReturnArchiveFromJObject.GetArchive<DeathEvent>(request.Content), ""),
                                "Adoption" => _archiveGenerator.GetAdoptionArchivePreview(ReturnArchiveFromJObject.GetArchive<AdoptionEvent>(request.Content), ""),
                                "Divorce" => _archiveGenerator.GetDivorceArchivePreview(ReturnArchiveFromJObject.GetArchive<DivorceEvent>(request.Content), ""),
                                "Marriage" => _archiveGenerator.GetMarriageArchivePreview(ReturnArchiveFromJObject.GetArchive<MarriageEvent>(request.Content), ""),
                            },
                TemplateId = await _mediator.Send(new GetCertificateTemplatesByNameQuery { Name = request.EventType  + " " + "Archive" })
            };
        }

    }
}