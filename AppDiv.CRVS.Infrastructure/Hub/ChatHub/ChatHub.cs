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
using Microsoft.AspNetCore.Cors;




namespace AppDiv.CRVS.Infrastructure.Hub.ChatHub;
[EnableCors("socketPolicy")]
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
        Console.WriteLine($"connected ");
        var userId = _userResolverService.GetUserId();
        Console.WriteLine($"connected {userId}");

        var userAddressId = _userResolverService.GetWorkingAddressId().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, userAddressId);
        await Clients.Group(userAddressId).UserConnected(userId);
        if(string.IsNullOrEmpty(userId)){
        _dbContext.OnlineUsers.Add(new OnlineUser
        {
            UserId = userId,
        });
        await _dbContext.SaveChangesAsync();
        }
       
        return base.OnConnectedAsync();
    }
    public override async Task<Task> OnDisconnectedAsync(Exception? exception)
    {
        var userId = _userResolverService.GetUserId();
        var userAddressId = _userResolverService.GetWorkingAddressId().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, userAddressId);
        await Clients.Group(userAddressId).UserDisconnected(userId);
         var onlineUser = await _dbContext.OnlineUsers.Where(u => u.UserId == userId).FirstOrDefaultAsync();
         if(onlineUser !=null){
            _dbContext.OnlineUsers.Remove(onlineUser);
            await _dbContext.SaveChangesAsync();
         }

        return base.OnDisconnectedAsync(exception);
    }

}