using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Domain.Repositories;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
{
    public class CreateCertificateTemplateCommandHandler : IRequestHandler<CreateCertificateTemplateCommand, object>
    {
        private readonly ICertificateTemplateRepository _certificateTemplateRepository;
        private readonly ILogger<CreateCertificateTemplateCommandHandler> _logger;
        public CreateCertificateTemplateCommandHandler(ICertificateTemplateRepository certificateTemplateRepository, ILogger<CreateCertificateTemplateCommandHandler> logger)
        {
            _certificateTemplateRepository = certificateTemplateRepository;
            _logger = logger;
        }
        public async Task<object> Handle(CreateCertificateTemplateCommand request, CancellationToken cancellationToken)
        {
            var certificateTemplate = new CertificateTemplate
            {
                CertificateType = request.CertificateTemplate.CertificateType,

            };
            try
            {
                var certificateId = await _certificateTemplateRepository.Add(certificateTemplate);
                var file = request.CertificateTemplate.SvgFile;
                var folderName = Path.Combine("Resources", "CertificateTemplates", $"{request.CertificateTemplate.CertificateType}");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = $"{certificateId}{fileExtension}";
                    var fullPath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                return new CreateCertificateTemplateResponse();



            }
            catch (System.Exception)
            {

                throw;
            }




        }
    }
}
