using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Import
{
    public class ImportAdderssCommandHandler : IRequestHandler<ImportAdderssCommand, ImportAdderssCommandResponse>
    {
        private readonly IAddressLookupRepository _AddressRepository;
        private readonly IConvertFileToObjectService _fileConvertor;
        private readonly ILogger<ImportAdderssCommandHandler> _logger;
        public ImportAdderssCommandHandler(IConvertFileToObjectService fileConvertor, IAddressLookupRepository AddresslookupRepository, ILogger<ImportAdderssCommandHandler> logger)
        {
            _AddressRepository = AddresslookupRepository;
            _fileConvertor = fileConvertor;
            _logger = logger;
        }
        public async Task<ImportAdderssCommandResponse> Handle(ImportAdderssCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var ImportAddressCommadResponse = new ImportAdderssCommandResponse();

            var validator = new ImportAdderssCommandValidator(_AddressRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                ImportAddressCommadResponse.Success = false;
                ImportAddressCommadResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    ImportAddressCommadResponse.ValidationErrors.Add(error.ErrorMessage);
                ImportAddressCommadResponse.Message = ImportAddressCommadResponse.ValidationErrors[0];
            }
            // _logger.LogCritical(request.Address.Code);
            if (ImportAddressCommadResponse.Success)
            {
                await _fileConvertor.ConvertFileToObject(request.Address, request.ParentAddressId, request.AreaTypeId, cancellationToken);
                // var addresses = CustomMapper.Mapper.Map<ICollection<Address>>(request);
                // await _AddressRepository.Import(addresses, cancellationToken);
                // await _AddressRepository.SaveChangesAsync(cancellationToken);        
            }
            return ImportAddressCommadResponse;
        }
    }
}
