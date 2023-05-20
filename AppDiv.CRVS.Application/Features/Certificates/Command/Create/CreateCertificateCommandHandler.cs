using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Service;

namespace AppDiv.CRVS.Application.Features.Certificates.Command.Create
{

    public class CreateCertificateCommandHandler : IRequestHandler<CreateCertificateCommand, CreateCertificateCommandResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<CreateCertificateCommand> log;
        public CreateCertificateCommandHandler(ICertificateRepository certificateRepository,
                                                ILogger<CreateCertificateCommand> log)
        {
            this.log = log;
            _certificateRepository = certificateRepository;
        }
        public async Task<CreateCertificateCommandResponse> Handle(CreateCertificateCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var createCertificateCommandResponse = new CreateCertificateCommandResponse();

            var validator = new CreateCertificateCommandValidator(_certificateRepository, log);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                createCertificateCommandResponse.Success = false;
                createCertificateCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    createCertificateCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                createCertificateCommandResponse.Message = createCertificateCommandResponse.ValidationErrors[0];
            }
            if (createCertificateCommandResponse.Success)
            {

                var Certificate = CustomMapper.Mapper.Map<Certificate>(request.Certificate);

                await _certificateRepository.InsertAsync(Certificate, cancellationToken);
                var result = await _certificateRepository.SaveChangesAsync(cancellationToken);

                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // createCustomerCommandResponse.Customer = customerResponse;          
            }
            return createCertificateCommandResponse;
        }
    }
}
