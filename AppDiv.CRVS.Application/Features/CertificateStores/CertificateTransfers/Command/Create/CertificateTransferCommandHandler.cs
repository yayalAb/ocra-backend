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

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var createPaymentCommandResponse = new CreateCertificateTransferCommandResponse();

            var validator = new CreateCertificateTransferCommandValidator(_CertificateTransferRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                createPaymentCommandResponse.Success = false;
                createPaymentCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    createPaymentCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                createPaymentCommandResponse.Message = createPaymentCommandResponse.ValidationErrors[0];
            }
            if (createPaymentCommandResponse.Success)
            {
                var CertificateTransfer = CustomMapper.Mapper.Map<CertificateSerialTransfer>(request.CertificateTransfer);
                Guid? recieverAddress = _userRepository.GetAll()
                                                        .Where(u => u.Id == CertificateTransfer.RecieverId)
                                                        .FirstOrDefault()?.AddressId;
                CertificateTransfer.Status = false;
                await _CertificateTransferRepository.InsertWithRangeAsync(CertificateTransfer, cancellationToken);
                var result = await _CertificateTransferRepository.SaveChangesAsync(cancellationToken);
                // await _CertificateTransferRepository.InsertWithRangeAsync(_logger, CertificateTransfer, cancellationToken);

            }
            return createPaymentCommandResponse;
        }
    }
}
