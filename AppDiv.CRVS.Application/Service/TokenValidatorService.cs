using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AppDiv.CRVS.Application.Service
{

    public class TokenValidatorService : ITokenValidatorService
    {
        private readonly IRevocationTokenRepository _tokenRepository;


        public TokenValidatorService(IRevocationTokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<bool> ValidateAsync(JwtSecurityToken token)
        {
            var httpContext = new HttpContextAccessor().HttpContext;
            httpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValue);
            var tokenId = headerValue.FirstOrDefault();
            var expiredToken = _tokenRepository.GetAll().Where(x => x.ExpirationDate <= DateTime.Now);
            Console.WriteLine("Checking !!");
            if (expiredToken.FirstOrDefault() != null)
            {
                Console.WriteLine("deleted !!");
                await _tokenRepository.DeleteAsync(expiredToken);
                _tokenRepository.SaveChanges();
            }

            if (string.IsNullOrEmpty(tokenId))
            {
                return true;
            }
            var tokenExists = _tokenRepository.GetAll().Where(x => x.Token == tokenId).FirstOrDefault();
            if (tokenExists == null)
            {
                return true;
            }
            return false;
        }
    }
}