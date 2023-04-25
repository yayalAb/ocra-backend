// using AppDiv.CRVS.Domain.Entities;
// using MediatR;
// using AppDiv.CRVS.Application.Interfaces.Persistence;
// using Microsoft.Extensions.Logging;
// using AppDiv.CRVS.Domain.Repositories;

// namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
// {
//     public class CreateCertificateTemplateCommandHandler : IRequestHandler<CreateCertificateTemplateCommand, CreateCertificateTemplateResponse>
//     {
//     //     private readonly ICertificateTemplateRepository _certificateTemplateRepository;
//     //     private readonly ILogger<CreateCertificateTemplateCommandHandler> _logger;
//     //     public CreateCertificateTemplateCommandHandler(ICertificateTemplateRepository certificateTemplateRepository, ILogger<CreateCertificateTemplateCommandHandler> logger)
//     //     {
//     //         _certificateTemplateRepository = certificateTemplateRepository;
//     //         _logger = logger;
//     //     }
//     //     public async Task<CreateCertificateTemplateResponse> Handle(CreateCertificateTemplateCommand request, CancellationToken cancellationToken)
//     //     {
//     //         var certificateTemplate = new CertificateTemplate
//     //         {
//     //             CertificateType = request.CertificateTemplate.CertificateType,

//     //         }

//     //             var file = request.CertificateTemplate.SvgFile;
//     //         var folderName = Path.Combine("Resources", "CertificateTemplates", $"{request.}");
//     //         var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
//     //         var dbPath = "";

//     //         if (file.Length > 0)
//     //         {
//     //             var date = request.GeneratedDate == null ? DateTime.Now.ToString("yyyyMMddHHmmss") : request.GeneratedDate?.ToString("yyyyMMddHHmmss");
//     //             var fileName = $"{request.OperationId}-{request.DocumentationId}-{date}.svg";
//     //             var fullPath = Path.Combine(pathToSave, fileName);
//     //             dbPath = Path.Combine(folderName, fileName);
//     //             using (var stream = new FileStream(fullPath, FileMode.Create))
//     //             {
//     //                 file.CopyTo(stream);
//     //             }
//     //         }


//     //     }
//     // }
// }
