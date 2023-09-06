using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Utility.Contracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
namespace AppDiv.CRVS.Infrastructure.Hub;
[EnableCors("socketPolicy")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

public class MessageHub : Hub<IMessageHubClient>
{
    private readonly ILogger<MessageHub> logger;
    private readonly IUserResolverService _userResolverService;
    private readonly CRVSDbContext _dbContext;

    public MessageHub(ILogger<MessageHub> logger, IUserResolverService userResolverService, CRVSDbContext dbContext)
    {
        this.logger = logger;
        _userResolverService = userResolverService;
        _dbContext = dbContext;
    }
    // public async Task SendNotification(List<NotificationResponseDTO> notifications)
    // {

    //     await Clients.All.SendNotification(notifications);

    // }
    // public async Task SendSingleNotification(NotificationResponseDTO notification)
    // {

    //     // // await Clients.All.SendSingleNotification(notification);
    //     logger.LogCritical("sending to group....");
    //     logger.LogCritical(notification.GroupId.ToString());
    //     await Clients.Group(notification.GroupId.ToString()).SendSingleNotification(notification);
    // }
    public override async Task<Task> OnConnectedAsync()
    {
        var userGroupIds = GetUserGroups();
        var workingAddressId = _userResolverService.GetWorkingAddressId();
        if (workingAddressId != Guid.Empty)
        {
            foreach (var groupId in userGroupIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString() + "_" + workingAddressId.ToString());
            Console.WriteLine($"Notification:conected to group ----- { groupId.ToString() + "_" + workingAddressId.ToString()}");
            }

            logger.LogCritical("connected");
        }


        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogCritical("disconnected");
        return base.OnDisconnectedAsync(exception);
    }
    private List<Guid> GetUserGroups()
    {
        var personId = _userResolverService.GetUserPersonalIdFromAccessTokenParam();
        var userGroups = _dbContext.Users.Where(u => u.PersonalInfoId == personId)
                    .Include(u => u.UserGroups)
                    .Select(u => u.UserGroups.Select(ug => ug.Id).ToList()).FirstOrDefault();
        if (userGroups == null)
        {
            throw new NotFoundException("logged in user not found ");
        }
        return userGroups;

    }
}