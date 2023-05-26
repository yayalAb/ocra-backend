using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Update
{
    public class UpdateCertificateTemplateCommandHandler : IRequestHandler<UpdateCertificateTemplateCommand, object>
    {
        private readonly ICertificateTemplateRepository _certificateTemplateRepository;
        private readonly ILogger<UpdateCertificateTemplateCommandHandler> _logger;
        private readonly IFileService _fileService;

        public UpdateCertificateTemplateCommandHandler(ICertificateTemplateRepository certificateTemplateRepository, ILogger<UpdateCertificateTemplateCommandHandler> logger, IFileService fileService)
        {
            _certificateTemplateRepository = certificateTemplateRepository;
            _logger = logger;
            _fileService = fileService;
        }
        public async Task<object> Handle(UpdateCertificateTemplateCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var certificate = await _certificateTemplateRepository.GetAsync(request.Id);
                if (certificate == null)
                {
                    throw new NotFoundException($"certificateTemplate with id {request.Id} is not found");
                }
                certificate.ModifiedAt = DateTime.Now;
                await _certificateTemplateRepository.UpdateAsync(certificate, x => x.Id);
                await _certificateTemplateRepository.SaveChangesAsync(cancellationToken);
                var file = request.SvgFile;
                var folderName = Path.Combine("Resources", "CertificateTemplates");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{request.Id}{fileExtension}";

                _fileService.UploadFormFile(file, fileName, pathToSave, FileMode.Create);

                return new UpdateCertificateTemplateResponse
                {
                    Message = "certificate template updated successfully"
                };

            }
            catch (System.Exception)
            {

                throw;
            }




        }
    }
}
