using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressByParent;
using AppDiv.CRVS.Application.Features.Search;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SearchController : ApiControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<SearchController> _Ilog;
        public SearchController(ISender mediator, ILogger<SearchController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog;
        }
        [HttpGet]
        [Route("SearchPersonalInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> SearchPersonalInfo([FromQuery] string SearchString, string? gender, int age)
        {
            // return new object{};
            return await _mediator.Send(new GetPersonalInfoQuery { SearchString = SearchString, gender = gender, age = age });
        }
        [HttpGet]
        [Route("SearchSimilarPersonalInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchSimilarPersonalInfo([FromQuery] SearchSimilarPersonalInfoQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
        [HttpGet]
        [Route("GetPersonalInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> Get([FromQuery] Guid Id)
        {
            return await _mediator.Send(new GetPersonalInfoById { Id = Id });
        }

        [HttpGet]
        [Route("GetPersonString")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<GetPersonByIdString> GetPrivew([FromQuery] Guid Id)
        {
            return await _mediator.Send(new getPersonForPrivewQuery { Id = Id });
        }



        [HttpGet]
        [Route("SearchCertificate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> SearchCertificate([FromQuery] string SearchString)
        {
            return await _mediator.Send(new SearchCertificateQuery { SearchString = SearchString });
        }

        [HttpGet]
        [Route("SearchEvents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> SearchEvents([FromQuery] string SearchString)
        {
            return await _mediator.Send(new SearchEventQuery { SearchString = SearchString });
        }
        [HttpGet]
        [Route("SearchPaymentExamptionRequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> searchPaymentExamptionRequest([FromQuery] GetPaymentExamptionRequestQuery query)
        {
            return await _mediator.Send(query);
        }
        [HttpGet]
        [Route("SearchApplicationUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> SearchApplicationUser([FromQuery] SearchUserDropDownQuery query)
        {
            return await _mediator.Send(query);
        }
        [HttpGet]
        [Route("GetParentInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> GetParentInfo([FromQuery] GetParentInfoByPersonIdQuery query)
        {
            return await _mediator.Send(query);
        }
        [HttpPost]
        [Route("reIndexPersonalInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> ReIndexPersonalInfo()
        {
            return await _mediator.Send(new ReIndexPersonalInfoCommand { });
        }
        [HttpPost]
        [Route("reIndexCertificate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> ReIndexCertificate()
        {
            return await _mediator.Send(new ReIndexCertificateCommand { });
        }
    }
}

