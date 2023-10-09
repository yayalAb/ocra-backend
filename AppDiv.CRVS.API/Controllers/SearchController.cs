using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressByParent;
using AppDiv.CRVS.Application.Features.Search;
using AppDiv.CRVS.Infrastructure.Service;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Nest;

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
        private readonly IElasticClient _elasticClient;
        private readonly IBackgroundJobs _bgjobs;

        public SearchController(ISender mediator, ILogger<SearchController> Ilog, IElasticClient elasticClient, IBackgroundJobs bgjobs)
        {
            _mediator = mediator;
            _Ilog = Ilog;
            _elasticClient = elasticClient;
            _bgjobs = bgjobs;
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
        public async Task<object> SearchCertificate([FromQuery] SearchCertificateQuery query)
        {
            return await _mediator.Send(query);
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
        [HttpPost]
        [Route("test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> testUpdage()
        {
            await _bgjobs.Test();
            return true;
            // var res = _elasticClient.Delete<PersonalInfoIndex>(DocumentPath<PersonalInfoIndex>.Id(personalInfo.Id.ToString()).Index("personal_info_second"));
            // var ress = _elasticClient.IndexDocument(new PersonalInfoIndex
            // {
            //     Id = personalInfo.Id.ToString(),
            //     FirstNameOr = "sample",
            //     MiddleNameOr = "father",
            //     LastNameOr = "gFather",
            //     NationalId = personalInfo.NationalId,
            //     PhoneNumber = personalInfo.PhoneNumber,
            // });

            // var ress = await _elasticClient.IndexAsync(new PersonalInfoIndex
            // {
            //     Id = personalInfo.Id.ToString(),
            //     FirstNameOr = "sample",
            //     MiddleNameOr = "father",
            //     LastNameOr = "gFather",
            //     NationalId = personalInfo.NationalId,
            //     PhoneNumber = personalInfo.PhoneNumber,
            // }, i => i.Index("personal_info_second"));

            // var task = await _elasticClient.UpdateByQueryAsync<PersonalInfoIndex>(c =>
            //                c.Index("personal_info_second")
            //                    .Query(q =>
            //                        q.Match(m => m.Field(f => f.Id.ToString()).Query(personalInfo.Id.ToString()))
            //                        ).Size(1)
            //                    .Script(script => script
            //                        .Source($"ctx._source = params.newDocument")

            //                        .Params(p => p.Add("newDocument", new PersonalInfoIndex
            //                        {
            //                            Id = personalInfo.Id.ToString(),
            //                            FirstNameOr = "sample",
            //                            MiddleNameOr = "father",
            //                            LastNameOr = "gFather",
            //                            NationalId = personalInfo.NationalId,
            //                            PhoneNumber = personalInfo.PhoneNumber,
            //                        }))
            //                    )
            //                );


            // var upData = new PersonalInfoIndex
            // {
            //     Id = personalInfo.Id.ToString(),
            //     FirstNameOr = "sample",
            //     MiddleNameOr = "father",
            //     LastNameOr = "gFather",
            //     NationalId = personalInfo.NationalId,
            //     PhoneNumber = personalInfo.PhoneNumber,
            // };
            // var updateres = await _elasticClient.UpdateAsync<PersonalInfoIndex>(DocumentPath<PersonalInfoIndex>
            //     .Id(personalInfo.Id.ToString()),
            //     p => p.Index("personal_info_second")
            //             .Doc(upData)
            //             .Refresh(Elasticsearch.Net.Refresh.True)
            //             );

            // var res22 = await _elasticClient.Indices.RefreshAsync("personal_info_second");

            // return true;
        }



    }
}

