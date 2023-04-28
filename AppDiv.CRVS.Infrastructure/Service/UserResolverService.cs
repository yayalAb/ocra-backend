
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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

        public string GetSelectedLanguage()
        {
            if (httpContext.HttpContext != null && httpContext.HttpContext.Request.Query.ContainsKey("lang"))
            {
                _logger.LogCritical(httpContext.HttpContext.Request.Query["lang"]);
                return httpContext.HttpContext.Request.Query["lang"];
            }
            return "en";
        }
    }
}
