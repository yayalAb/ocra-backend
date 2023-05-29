
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

        public string GetUserEmail()
        {
            return httpContext.HttpContext.User?.Claims?.SingleOrDefault(p => p.Type == "Email")?.Value;
        }

        public Guid GetUserId()
        {
            var guid = httpContext.HttpContext?.User?.Claims?.SingleOrDefault(p => p.Type == "UserId")?.Value;
            if (guid == null || guid == string.Empty)
            {
                return Guid.Empty;
            }
            return new Guid(guid);
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
