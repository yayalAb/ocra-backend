using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
{
    public class CreateAdderssCommandHandler : IRequestHandler<CreateAdderssCommand, CreateAdderssCommandResponse>
    {
        private readonly IAddressLookupRepository _AddressRepository;
        private readonly ILogger<CreateAdderssCommandHandler> _logger;
        public CreateAdderssCommandHandler(IAddressLookupRepository AddresslookupRepository, ILogger<CreateAdderssCommandHandler> logger)
        {
            _AddressRepository = AddresslookupRepository;
            _logger = logger;
        }
        public async Task<CreateAdderssCommandResponse> Handle(CreateAdderssCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var CreateAddressCommadResponse = new CreateAdderssCommandResponse();

            var validator = new CreateAdderssCommandValidator(_AddressRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                CreateAddressCommadResponse.Success = false;
                CreateAddressCommadResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateAddressCommadResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateAddressCommadResponse.Message = CreateAddressCommadResponse.ValidationErrors[0];
            }
            _logger.LogCritical(request.Address.Code);
            if (CreateAddressCommadResponse.Success)
            {
                //can use this instead of automapper

                var Address = new Address
                {
                    Id = Guid.NewGuid(),
                    AddressName = request.Address.AddressName,
                    StatisticCode = request.Address.StatisticCode,
                    Code = request.Address.Code,
                    CodePerfix = request.Address.CodePerfix,
                    CodePostfix = request.Address.CodePostfix,
                    AdminLevel = request.Address.AdminLevel,
                    AreaTypeLookupId = request.Address.AreaTypeLookupId,
                    ParentAddressId = request.Address.ParentAddressId,
                    AdminTypeLookupId = request.Address.AdminTypeLookupId
                };
                //
                await _AddressRepository.InsertAsync(Address, cancellationToken);
                await _AddressRepository.SaveChangesAsync(cancellationToken);
                CreateAddressCommadResponse.Id = Address.Id;
                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // CreateLookupCommadResponse.Customer = customerResponse;          
            }
            return CreateAddressCommadResponse;
        }
    }
}
