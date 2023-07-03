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

namespace AppDiv.CRVS.Application.Features.Lookups.Command.Create
{

    public class CreateLookupCommandHandler : IRequestHandler<CreateLookupCommand, CreateLookupCommadResponse>
    {
        private readonly ILookupRepository _lookupRepository;
        private readonly ILookupCouchRepository _lookupCouchRepository;

        public CreateLookupCommandHandler(ILookupRepository lookupRepository, ILookupCouchRepository lookupCouchRepository)
        {
            _lookupRepository = lookupRepository;
            _lookupCouchRepository = lookupCouchRepository;
        }
        public async Task<CreateLookupCommadResponse> Handle(CreateLookupCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var CreateLookupCommadResponse = new CreateLookupCommadResponse();

            var validator = new CreateLookupCommandValidator(_lookupRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                CreateLookupCommadResponse.Success = false;
                CreateLookupCommadResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateLookupCommadResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateLookupCommadResponse.Message = CreateLookupCommadResponse.ValidationErrors[0];
            }
            if (CreateLookupCommadResponse.Success)
            {
                //can use this instead of automapper
                var lookup = new Lookup
                {
                    Id = Guid.NewGuid(),
                    Key = request.lookup.Key,
                    Value = request.lookup.Value,
                    Description = request.lookup.Description,
                    StatisticCode = request.lookup.StatisticCode,
                    Code = request.lookup.Code
                };
                //
                await _lookupRepository.InsertAsync(lookup, cancellationToken);
                // await _lookupCouchRepository.InsertLookupAsync(lookup);

                var result = await _lookupRepository.SaveChangesAsync(cancellationToken);

                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // CreateLookupCommadResponse.Customer = customerResponse;          
            }
            return CreateLookupCommadResponse;
        }
    }
}
