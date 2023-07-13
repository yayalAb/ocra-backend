using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Utility.Contracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AppDiv.CRVS.Domain.Entities;



namespace AppDiv.CRVS.Infrastructure.Hub.ChatHub;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub<IChatHubClient>
{
    private readonly ILogger<ChatHub> logger;
    private readonly IUserResolverService _userResolverService;
    private readonly CRVSDbContext _dbContext;

    public ChatHub(ILogger<ChatHub> logger, IUserResolverService userResolverService, CRVSDbContext dbContext)
    {
        this.logger = logger;
        _userResolverService = userResolverService;
        _dbContext = dbContext;
    }
    public async Task SendChatAsync(MessageDTO message)
    {
        await Clients.User(message.SenderId).SendChatAsync(message);
        await Clients.User(message.ReceiverId).SendChatAsync(message);
    }
    public override async Task<Task> OnConnectedAsync()
    {
        var userGroupIds = GetUserGroups();
        foreach (var groupId in userGroupIds)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
            // await Clients.Group(groupId.ToString()).SendAsync("userConnected", _userResolverService.GetUserId());
        }



        logger.LogCritical("connected");
        Console.WriteLine($"Notification:----- {_userResolverService.GetUserId()}");


        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userGroupIds = GetUserGroups();
        // foreach (var groupId in userGroupIds)
        // {
        //     await Clients.Group(groupId.ToString()).SendAsync("userDisConnected", _userResolverService.GetUserId());
        // }

        logger.LogCritical("disconnected");
        return base.OnDisconnectedAsync(exception);
    }
    private List<Guid> GetUserGroups()
    {
        var personId = _userResolverService.GetUserPersonalId();
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