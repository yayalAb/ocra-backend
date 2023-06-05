using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Create
{

    public class CreateCertificateTransferCommandHandler : IRequestHandler<CreateCertificateTransferCommand, CreateCertificateTransferCommandResponse>
    {
        private readonly ICertificateTransferRepository _CertificateTransferRepository;
        private readonly ICertificateRangeRepository _certificateRangeRepository;
        private readonly IUserRepository _userRepository;
        public CreateCertificateTransferCommandHandler(ICertificateTransferRepository CertificateTransferRepository,
                                                        ICertificateRangeRepository certificateRangeRepository,
                                                        IUserRepository userRepository)
        {
            _CertificateTransferRepository = CertificateTransferRepository;
            _certificateRangeRepository = certificateRangeRepository;
            _userRepository = userRepository;
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

                await _CertificateTransferRepository.InsertWithRangeAsync(CertificateTransfer, cancellationToken);
                var result = await _CertificateTransferRepository.SaveChangesAsync(cancellationToken);

            }
            return createPaymentCommandResponse;
        }
    }
}
