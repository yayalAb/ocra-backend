using System.Linq;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Persistence.Couch;

namespace AppDiv.CRVS.Application.Features.Lookups.Command.Import
{

    public class ImportLookupCommandHandler : IRequestHandler<ImportLookupCommand, ImportLookupCommadResponse>
    {
        private readonly ILookupRepository _lookupRepository;
        private readonly ILookupCouchRepository _lookupCouchRepository;

        public ImportLookupCommandHandler(ILookupRepository lookupRepository, ILookupCouchRepository lookupCouchRepository)
        {
            _lookupRepository = lookupRepository;
            _lookupCouchRepository = lookupCouchRepository;
        }
        public async Task<ImportLookupCommadResponse> Handle(ImportLookupCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var response = new ImportLookupCommadResponse();

            var validator = new ImportLookupCommandValidator(_lookupRepository);
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
            if (response.Success)
            {
                //can use this instead of automapper
                var lookups = CustomMapper.Mapper.Map<ICollection<Lookup>>(request.Lookups);

                await _lookupRepository.Import(lookups, cancellationToken);
                await _lookupRepository.SaveChangesAsync(cancellationToken);
                    
            }
            return response;
        }
    }
}
