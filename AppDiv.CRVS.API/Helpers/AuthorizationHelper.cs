using System.Security.Claims;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.API.Helpers;
public class AuthorizationHelper
{
    private readonly IIdentityService _identityService;

    public AuthorizationHelper(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<bool> checkRole(string page, string controllerAction, string controllerName)
    {
        var httpContext = new HttpContextAccessor().HttpContext;
        var userId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {

            return false;
        }
        var roles = await _identityService.GetUserRoles(userId);

        var role = roles.Where(r =>
              controllerName.ToLower() == "lookup"
                        ? r.page.ToLower().Contains(page.ToLower())
                        : r.page.ToLower() == page.ToLower()).FirstOrDefault();
        bool isAuthorized = false;

        if (role == null)
        {

            return false;
        }

        switch (controllerAction)
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
        return isAuthorized;


    }
}