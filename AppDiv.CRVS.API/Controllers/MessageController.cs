
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Messages.Query.GetMessagesByUserId;
using AppDiv.CRVS.Application.Features.Messages.Command.Create;


using Microsoft.AspNetCore.SignalR;
using AppDiv.CRVS.Infrastructure.Hub.ChatHub;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Features.User.Query.GetUsersByAddressId;

namespace AppDiv.CRVS.API.Controllers
{
    public class MessageController : ApiControllerBase
    {
        private readonly IHubContext<ChatHub, IChatHubClient> _chatHub;
        private readonly IUserResolverService _userResolverService;


        public MessageController(IHubContext<ChatHub, IChatHubClient> chatHub, IUserResolverService userResolverService)
        {
            _chatHub = chatHub;
            _userResolverService = userResolverService;
        }

        [HttpGet("allMessages")]
        public async Task<IActionResult> GetAllMessageByUserId()
        {
            var userId = _userResolverService.GetUserId();
            if (userId == null)
            {
                return Unauthorized("please login first");
            }
            return Ok(await Mediator.Send(new GetMessagesByUserIdQuery { UserId = _userResolverService.GetUserId() }));
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] CreateMessageCommand command)
        {
            var res = await Mediator.Send(command);

            if (res.Success)
            {
                await _chatHub.Clients.User(res.CreatedMessage.SenderId).NewMessage(res.CreatedMessage);
                await _chatHub.Clients.User(res.CreatedMessage.ReceiverId).NewMessage(res.CreatedMessage);
                res.CreatedMessage = null;
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }

        }
        [HttpGet("Contacts")]
        public async Task<IActionResult> GetContacts()
        {
            var userAddress = _userResolverService.GetWorkingAddressId();
            var userId = _userResolverService.GetUserId();
            Console.WriteLine($"addressId ----{userAddress}");
            if (userAddress == null || userAddress == Guid.Empty)
            {
                return Unauthorized("Could not find user address ,please login first");
            }
            return Ok(await Mediator.Send(new GetUsersByAddressIdQuery { AddressId = userAddress, Except = userId, AddOnlineFlag = true }));
        }
        [HttpPost("test")]
        public async Task<IActionResult> soket([FromBody] CreateMessageCommand command)
        {
            var res = await Mediator.Send(command);

            await _chatHub.Clients.User(res.CreatedMessage.SenderId).NewMessage(res.CreatedMessage);
            await _chatHub.Clients.User(res.CreatedMessage.ReceiverId).NewMessage(res.CreatedMessage);
            return Ok("message sent");
        }
    }
}