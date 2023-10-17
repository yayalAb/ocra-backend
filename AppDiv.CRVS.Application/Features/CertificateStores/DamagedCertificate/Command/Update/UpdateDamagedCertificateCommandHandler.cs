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

    public class UpdateDamagedCertificatesCommandHandler : IRequestHandler<UpdateDamagedCertificatesCommand, UpdateDamagedCertificatesCommandResponse>
    {
        private readonly ICertificateRangeRepository _certificateRangeRepository;
        private readonly IUserResolverService _userResolver;

        public UpdateDamagedCertificatesCommandHandler(ICertificateRangeRepository certificateRangeRepository, IUserResolverService userResolver)
        {
            _certificateRangeRepository = certificateRangeRepository;
            this._userResolver = userResolver;
        }
        public async Task<UpdateDamagedCertificatesCommandResponse> Handle(UpdateDamagedCertificatesCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateDamagedCertificatesCommandResponse();

            var validator = new UpdateDamagedCertificatesCommandValidator(_certificateRangeRepository);
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
                    var damagedCertificateRange = CustomMapper.Mapper.Map<CertificateSerialRange>(request);
                    damagedCertificateRange.IsDamaged = true;
                    damagedCertificateRange.AddressId = _userResolver.GetWorkingAddressId();
                    damagedCertificateRange.UserId = _userResolver.GetUserId();
                    // save the date
                    await _certificateRangeRepository.UpdateAsync(damagedCertificateRange, c => c.Id);
                    await _certificateRangeRepository.SaveChangesAsync(cancellationToken);
                    response.Updated("Damaged Certificate"); // set the response to success
                }
                catch (System.Exception)
                {
                    response.BadRequest("Something Went wrong...");
                    // throw;
                }

            }
            return response;
        }
    }
}
