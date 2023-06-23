using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Create
{

    public class CreateCertificateTransferCommandHandler : IRequestHandler<CreateCertificateTransferCommand, CreateCertificateTransferCommandResponse>
    {
        private readonly ICertificateTransferRepository _CertificateTransferRepository;
        private readonly ICertificateRangeRepository _certificateRangeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateCertificateTransferCommandHandler> _logger;
        public CreateCertificateTransferCommandHandler(ICertificateTransferRepository CertificateTransferRepository,
                                                        ICertificateRangeRepository certificateRangeRepository,
                                                        ILogger<CreateCertificateTransferCommandHandler> logger,
                                                        IUserRepository userRepository)
        {
            _CertificateTransferRepository = CertificateTransferRepository;
            _certificateRangeRepository = certificateRangeRepository;
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task<CreateCertificateTransferCommandResponse> Handle(CreateCertificateTransferCommand request, CancellationToken cancellationToken)
        {
            var response = new CreateCertificateTransferCommandResponse();

            var validator = new CreateCertificateTransferCommandValidator(_CertificateTransferRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                response.Success = false;
                response.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    response.ValidationErrors.Add(error.ErrorMessage);
                response.Message = response.ValidationErrors[0];
            }
            if (response.Success) // if there is no validation error 
            {
                try
                {
                    // map the request to the model
                    var CertificateTransfer = CustomMapper.Mapper.Map<CertificateSerialTransfer>(request.CertificateTransfer);
                    CertificateTransfer.Status = false;
                    // save the date
                    await _CertificateTransferRepository.InsertWithRangeAsync(CertificateTransfer, cancellationToken);
                    await _CertificateTransferRepository.SaveChangesAsync(cancellationToken);
                    response.Created("Certificate Transfer"); // set the response to success
                }
                catch (System.Exception)
                {
                    response.BadRequest("Unable to Transfer the Certificates...");
                    // throw;
                }

            }
            return response;
        }
    }
}
