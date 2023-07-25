using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Fingerprint.commands.ManageApikey
{
    // Customer create command with CustomerResponse
    public class UpdateKeysCommand : IRequest<FingerprintApiKey>
    {
        public string clientAPIKey { get; set; }
        public string clientKey { get; set; }
        public UpdateKeysCommand()
        {

        }
    }


    public class UpdateKeysCommandHandler : IRequestHandler<UpdateKeysCommand, FingerprintApiKey>
    {
        private readonly IFingerprintApiKeyRepostory _apikeyRepository;
        public UpdateKeysCommandHandler(IFingerprintApiKeyRepostory apikeyRepository)
        {
            _apikeyRepository = apikeyRepository;
        }
        public async Task<FingerprintApiKey> Handle(UpdateKeysCommand request, CancellationToken cancellationToken)
        {
            var ApiKey = _apikeyRepository.GetAll().FirstOrDefault();

            FingerprintApiKey ApikeyEntity = new FingerprintApiKey
            {
                Id = ApiKey.Id,
                clientAPIKey = request.clientAPIKey,
                clientKey = request.clientKey,
            };
            try
            {
                await _apikeyRepository.UpdateAsync(ApikeyEntity, x => x.Id);
                await _apikeyRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw;
            }
            return ApikeyEntity;
        }
    }
}