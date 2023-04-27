using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
{
    public class CreateCertificateTemplateCommandHandler : IRequestHandler<CreateCertificateTemplateCommand, object>
    {
        private readonly ICertificateTemplateRepository _certificateTemplateRepository;
        private readonly IFileService _FileService;
        private readonly ILogger<CreateCertificateTemplateCommandHandler> _logger;
        public CreateCertificateTemplateCommandHandler(ICertificateTemplateRepository certificateTemplateRepository, IFileService FileService,ILogger<CreateCertificateTemplateCommandHandler> logger)
        {
            _certificateTemplateRepository = certificateTemplateRepository;
            _FileService = FileService;
            _logger = logger;
        }
        public async Task<object> Handle(CreateCertificateTemplateCommand request, CancellationToken cancellationToken)
        {
            var certificateTemplate = new CertificateTemplate
            {
                CertificateType = request.CertificateType,

            };
            try
            {
                var certificateId = await _certificateTemplateRepository.Add(certificateTemplate);
                await _certificateTemplateRepository.SaveChangesAsync(cancellationToken);

                var file = request.SvgFile;
                var folderName = Path.Combine("Resources", "CertificateTemplates");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var fileName = $"{certificateId}{Path.GetExtension(file.FileName)}";

                _FileService.UploadFormFile(file,fileName,pathToSave,FileMode.Create);

                return new CreateCertificateTemplateResponse{
                    Message = "certificate template uploaded successfully"
                };



            }
            catch (System.Exception)
            {

                throw;
            }




        }
    }
}
