using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;


namespace AppDiv.CRVS.API.Helpers;
public class CustomAuthorizeAttribute : TypeFilterAttribute
{
    public CustomAuthorizeAttribute(string ControllerName, string ControllerAction)
    : base(typeof(AuthorizeActionFilter))
    {
        Arguments = new object[] { ControllerName, ControllerAction };
    }
}

public class AuthorizeActionFilter : IAuthorizationFilter
{
    private readonly string _ControllerName;
    private readonly string _ControllerAction;
    private readonly IIdentityService _identityService;

    public AuthorizeActionFilter(string ControllerName, string ControllerAction, IIdentityService identityService)
    {
        _ControllerName = ControllerName;
        _ControllerAction = ControllerAction;
        _identityService = identityService;
    }



    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            context.Result = new ForbidResult();
            return;
        }
        var roles = await _identityService.GetUserRoles(userId);
        var addressPages = new string[]{"country","region","zone","woreda","kebele"};
        var auditLogPages = new string[]{"userauditlog","eventauditlog", "workhistoryauditlog","systemauditlog","transactionaudit"};
        var userRoles = roles.Where(r => 
                        _ControllerName.ToLower() == "address"
                        ? addressPages.Contains(r.page.ToLower()) 
                        :_ControllerName.ToLower() == "auditlog"
                        ?auditLogPages.Contains(r.page.ToLower())
                        : r.page.ToLower() == _ControllerName.ToLower()).ToList();
        bool isAuthorized = false;

        if (userRoles == null || userRoles?.Count == 0)
        {
            context.Result = new ForbidResult();
            return;
        }

        switch (_ControllerAction)
        {
            case "ReadAll":
                isAuthorized = userRoles.Where(r => r.canView).Any(); break;
            case "ReadSingle":
                isAuthorized = userRoles.Where(r => r.canViewDetail).Any(); break;
            case "Update":
                isAuthorized = userRoles.Where(r => r.canUpdate).Any(); break;
            case "Delete":
                isAuthorized = userRoles.Where(r => r.canDelete).Any(); break;
            case "Add":
                isAuthorized =userRoles.Where(r => r.canAdd).Any(); break;
        }
        if (!isAuthorized)
        {
            context.Result = new ForbidResult();
        }


    }
}