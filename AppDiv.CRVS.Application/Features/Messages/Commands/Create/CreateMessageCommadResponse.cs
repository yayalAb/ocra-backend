using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Domain.Entities;


namespace AppDiv.CRVS.Application.Features.Messages.Command.Create
{
    public class CreateMessageCommadResponse : BaseResponse
    {
        public  Message CreatedMessage {get; set; }
        public CreateMessageCommadResponse() : base()
        {
        }

    }
}
