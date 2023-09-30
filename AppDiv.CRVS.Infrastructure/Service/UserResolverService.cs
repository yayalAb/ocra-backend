
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
// using AppDiv.CRVS.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
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
            try
            {
                // Console.WriteLine($"userrrr=============== {httpContext?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)}");

                var userId = httpContext?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {

                    var tokenstring = httpContext?.HttpContext?.Request == null ? null :
                     httpContext?.HttpContext?.Request?.Headers["Authorization"].ToString().Split(" ").Last();
                    if (!string.IsNullOrEmpty(tokenstring) && tokenstring.ToLower() != "undefined" && tokenstring.ToLower() != "null")
                    {
                        var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);

                        userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


                    }
                    else if (!string.IsNullOrEmpty(httpContext?.HttpContext?.Request.Query["access_token"]))
                    {
                        var tokenFromQueryParam = new JwtSecurityTokenHandler().ReadJwtToken(httpContext?.HttpContext?.Request.Query["access_token"]);
                        userId = tokenFromQueryParam.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    }

                }
                return userId;
            }
            catch (Exception)
            {
                return null;

            }

        }
        public Guid GetWorkingAddressId()
        {
            try
            {

                var tokenstring = httpContext?.HttpContext?.Request.Headers["Authorization"].ToString().Split(" ").Last();
                if (string.IsNullOrEmpty(tokenstring) || tokenstring.ToString().ToLower() == "undefined" || tokenstring.ToString().ToLower() == "null")
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
            catch (Exception)
            {
                return Guid.Empty;
            }
        }
        public int GetAdminLevel()
        {
            try
            {
                var tokenstring = httpContext?.HttpContext?.Request.Headers["Authorization"].ToString().Split(" ").Last();
                if (string.IsNullOrEmpty(tokenstring) || tokenstring.ToString().ToLower() == "undefined" || tokenstring.ToString().ToLower() == "null")

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
            catch (Exception)
            {
                return -1;
            }
        }
        public Guid GetUserPersonalId()
        {
            try
            {

                var tokenstring = httpContext?.HttpContext?.Request.Headers["Authorization"].ToString().Split(" ").Last();
                if (string.IsNullOrEmpty(tokenstring) || tokenstring.ToString().ToLower() == "undefined" || tokenstring.ToString().ToLower() == "null")

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
            catch (Exception e)
            {
                return Guid.Empty;
            }
        }
        public Guid GetUserPersonalIdFromAccessTokenParam()
        {
            try
            {

                var tokenstring = httpContext?.HttpContext?.Request.Query["access_token"];

                if (string.IsNullOrEmpty(tokenstring) || tokenstring.ToString().ToLower() == "undefined" || tokenstring.ToString().ToLower() == "null")
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
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        public Guid GetWorkingAddressIdFromAccessTokenParam()
        {
            try
            {

                var tokenstring = httpContext?.HttpContext?.Request.Query["access_token"];

                if (string.IsNullOrEmpty(tokenstring) || tokenstring.ToString().ToLower() == "undefined" || tokenstring.ToString().ToLower() == "null")
                {
                    return Guid.Empty;

                }
                var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);

                var personId = token.Claims.FirstOrDefault(c => c.Type == "addressId")?.Value;
                if (personId == null)
                {
                    return Guid.Empty;
                }
                return new Guid(personId);
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        public string GetLocale()
        {
            var httpContext = new HttpContextAccessor().HttpContext;
            if (httpContext != null && httpContext.Request.Headers.ContainsKey("lang"))
            {
                httpContext.Request.Headers.TryGetValue("lang", out StringValues headerValue);
                return headerValue.FirstOrDefault();
            }
            else
            {
                return "or";
            }
        }
    }
}
