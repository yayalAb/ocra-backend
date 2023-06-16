
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
// using AppDiv.CRVS.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure.Services
{
    public class UserResolverService : IUserResolverService

    {
        private readonly IHttpContextAccessor httpContext;
        private readonly ILogger<UserResolverService> _logger;


        public UserResolverService(IHttpContextAccessor httpContext, ILogger<UserResolverService> logger)
        {
            this.httpContext = httpContext;
            _logger = logger;
        }

        public string? GetUserEmail()
        {
            return httpContext?.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
        }

        public string? GetUserId()
        {

            return  httpContext?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
       
        }
        public Guid GetUserPersonalId()
        {
            var tokenstring = httpContext?.HttpContext?.Request.Headers["Authorization"].ToString().Split(" ").Last();
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);
            var personId = token.Claims.FirstOrDefault(c => c.Type == "personId")?.Value;
            if(personId == null){
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
