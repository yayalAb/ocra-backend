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
    public override async Task<Task> OnConnectedAsync()
    {
        // Console.WriteLine($"connected list ------ {Users}");
        var userId = _userResolverService.GetUserId();
        var userAddressId = _userResolverService.GetWorkingAddressId().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, userAddressId);
        await Clients.Group(userAddressId).UserConnected(userId);
        // _dbContext.OnlineUsers.Add(new OnlineUser
        // {
        //     UserId = userId,
        // });
        // await _dbContext.SaveChangesAsync();
        return base.OnConnectedAsync();
    }
    public override async Task<Task> OnDisconnectedAsync(Exception? exception)
    {
        var userId = _userResolverService.GetUserId();
        var userAddressId = _userResolverService.GetWorkingAddressId().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, userAddressId);
        await Clients.Group(userAddressId).UserDisconnected(userId);
        // _dbContext.OnlineUsers.FindAsync();
        return base.OnDisconnectedAsync(exception);
    }

}