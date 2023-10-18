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
        var role = roles.Where(r => 
                        _ControllerName.ToLower() == "address"
                        ? addressPages.Contains(r.page.ToLower()) 
                        :_ControllerName.ToLower() == "auditlog"
                        ?auditLogPages.Contains(r.page.ToLower())
                        :_ControllerName.ToLower() == "lookup"
                        ? r.page.ToLower().Contains("lookup")
                        : r.page.ToLower() == _ControllerName.ToLower()).FirstOrDefault();
        bool isAuthorized = false;

        if (role == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        switch (_ControllerAction)
        {
            case "ReadAll":
                isAuthorized = role.canView; break;
            case "ReadSingle":
                isAuthorized = role.canViewDetail; break;
            case "Update":
                isAuthorized = role.canUpdate; break;
            case "Delete":
                isAuthorized = role.canDelete; break;
            case "Add":
                isAuthorized = role.canAdd; break;
        }
        if (!isAuthorized)
        {
            context.Result = new ForbidResult();
        }


    }
}