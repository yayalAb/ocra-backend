
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
// using AppDiv.CRVS.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure.Services
{
    public class UserResolverService : IUserResolverService

    {
        private readonly IHttpContextAccessor httpContext;
        private readonly ILogger<UserResolverService> _logger;
        // private readonly CRVSDbContext _dbContext;

        public UserResolverService(IHttpContextAccessor httpContext, ILogger<UserResolverService> logger)
        {
            this.httpContext = httpContext;
            _logger = logger;
            // _dbContext = dbContext;
        }

        public string? GetUserEmail()
        {
            return httpContext?.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
        }

        public string? GetUserId()
        {
            // Console.WriteLine($"userrrr=============== {httpContext?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)}");
            var userId = httpContext?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var tokenstring = httpContext?.HttpContext?.Request == null ? null :
                 httpContext?.HttpContext?.Request?.Headers["Authorization"].ToString().Split(" ").Last();
                Console.WriteLine($"userrrr=============== {tokenstring}");

                if (!string.IsNullOrEmpty(tokenstring) && tokenstring.ToLower() != "undefined" && tokenstring.ToLower() != "null")
                {
                    // TODO: readtoken exception handling
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);

                    userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                }

            }
            return userId;

        }
        public Guid GetWorkingAddressId()
        {
            var tokenstring = httpContext?.HttpContext?.Request.Headers["Authorization"].ToString().Split(" ").Last();
            if (string.IsNullOrEmpty(tokenstring))
            {
                return Guid.Empty;

            }
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);

            var addressId = token.Claims.FirstOrDefault(c => c.Type == "addressId")?.Value;
            if (addressId == null)
            {
                return Guid.Empty;
            }
            return new Guid(addressId);
        }
        public int GetAdminLevel()
        {
            var tokenstring = httpContext?.HttpContext?.Request.Headers["Authorization"].ToString().Split(" ").Last();
            if (string.IsNullOrEmpty(tokenstring))
            {
                return -1;

            }
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);

            var adminLevel = token.Claims.FirstOrDefault(c => c.Type == "adminLevel")?.Value;
            if (int.TryParse(adminLevel, out int level))
            {
                return level;
            }
            return -1;
        }
        public Guid GetUserPersonalId()
        {
            var tokenstring = httpContext?.HttpContext?.Request.Headers["Authorization"].ToString().Split(" ").Last();
            if (string.IsNullOrEmpty(tokenstring))
            {
                return Guid.Empty;

            }
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);

            var personId = token.Claims.FirstOrDefault(c => c.Type == "personId")?.Value;
            if (personId == null)
            {
                return Guid.Empty;
            }
            return new Guid(personId);
        }
        public Guid GetUserPersonalIdFromAccessTokenParam()
        {
        
            var tokenstring = httpContext?.HttpContext?.Request.Query["access_token"];
            
            if (string.IsNullOrEmpty(tokenstring))
            {
                return Guid.Empty;

            }
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);

            var personId = token.Claims.FirstOrDefault(c => c.Type == "personId")?.Value;
            if (personId == null)
            {
                return Guid.Empty;
            }
            return new Guid(personId);
        }


        public string GetLocale()
        {
            if (httpContext.HttpContext != null && httpContext.HttpContext.Request.Query.ContainsKey("locale"))
            {
                return httpContext.HttpContext.Request.Query["locale"].ToArray()[0];
            }
            return string.Empty;
        }
    }
}
