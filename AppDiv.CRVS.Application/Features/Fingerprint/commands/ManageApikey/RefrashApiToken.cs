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
using System.Text.Json;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Fingerprint.commands.ManageApikey
{
    // Customer create command with CustomerResponse
    public class RefrashApiToken : IRequest<FingerprintApiKey>
    {
        public string clientAPIKey { get; set; }
        public string clientKey { get; set; }
        public RefrashApiToken()
        {

        }
    }

    public class RefrashApiTokenHandler : IRequestHandler<RefrashApiToken, FingerprintApiKey>
    {
        private readonly IFingerprintApiKeyRepostory _apikeyRepository;
        private readonly IRequestApiService _apiService;
        public RefrashApiTokenHandler(IFingerprintApiKeyRepostory apikeyRepository, IRequestApiService apiService)
        {
            _apikeyRepository = apikeyRepository;
            _apiService = apiService;
        }
        public async Task<FingerprintApiKey> Handle(RefrashApiToken request, CancellationToken cancellationToken)
        {
            var ApiKey = _apikeyRepository.GetAll().FirstOrDefault();
            string Token = "";
            using (var httpClient = new HttpClient())
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var LonginResponse = await httpClient.PostAsync("https://bioplugin.cloudabis.com/api/Authorizations/Token", content);
                LonginResponse.EnsureSuccessStatusCode();
                var responseBody = await LonginResponse.Content.ReadAsStringAsync();
                FingerprintApiLoginResponseDto responsData = JsonSerializer.Deserialize<FingerprintApiLoginResponseDto>(responseBody);
                if (responsData?.responseData?.accessToken != null)
                {
                    Token = responsData.responseData.accessToken;
                }
            }
            FingerprintApiKey ApikeyEntity = new FingerprintApiKey
            {
                Id = ApiKey.Id,
                clientAPIKey = request.clientAPIKey,
                clientKey = request.clientKey,
                token = ApiKey.token
            };

            if (!string.IsNullOrEmpty(Token))
            {
                ApikeyEntity.token = Token;
            }
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