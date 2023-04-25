using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Application.Exceptions;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Update
{
    public class UpdateCertificateTemplateCommandHandler : IRequestHandler<UpdateCertificateTemplateCommand, object>
    {
        private readonly ICertificateTemplateRepository _certificateTemplateRepository;
        private readonly ILogger<UpdateCertificateTemplateCommandHandler> _logger;
        public UpdateCertificateTemplateCommandHandler(ICertificateTemplateRepository certificateTemplateRepository, ILogger<UpdateCertificateTemplateCommandHandler> logger)
        {
            _certificateTemplateRepository = certificateTemplateRepository;
            _logger = logger;
        }
        public async Task<object> Handle(UpdateCertificateTemplateCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var certificate = await _certificateTemplateRepository.GetAsync(request.CertificateTemplate.Id);
                if(certificate == null){
                    throw new NotFoundException($"certificateTemplate with id {request.CertificateTemplate.Id} is not found");
                }
                var file = request.CertificateTemplate.SvgFile;
                var folderName = Path.Combine("Resources", "CertificateTemplates", $"{request.CertificateTemplate.CertificateType}");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = $"{request.CertificateTemplate.Id}{fileExtension}";
                    var fullPath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Truncate))
                    {
                        file.CopyTo(stream);
                    }
                }

                return new UpdateCertificateTemplateResponse();



            }
            catch (System.Exception)
            {

                throw;
            }




        }
    }
}
