
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.WorkHistorys.Query.GetById;

namespace AppDiv.CRVS.API.Controllers
{
    public class WorkHistoryController : ApiControllerBase
    {
        [HttpGet("GetUserWorkHistory/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> GetById(string id)
        {
            return await Mediator.Send(new WorkHistoryGetByIdQuery(id));
        }
    }
}