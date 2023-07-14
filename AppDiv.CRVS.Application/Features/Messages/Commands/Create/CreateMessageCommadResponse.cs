using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;


namespace AppDiv.CRVS.Application.Features.Messages.Command.Create
{
    public class CreateMessageCommadResponse : BaseResponse
    {
        public  MessageDTO CreatedMessage {get; set; }
        public CreateMessageCommadResponse() : base()
        {
        }

    }
}
