
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Search
{
    // Customer SearchSimilarPersonalInfoQuery with  response
    public class SearchSimilarPersonalInfoQuery : IRequest<object>
    {
        public string? FirstNameOr {get; set; }
        public string? FirstNameAm { get; set; }
        public string? MiddleNameOr { get; set; }
        public string? MiddleNameAm { get; set; }
        public string? LastNameOr { get; set; }
        public string? LastNameAm { get; set; }
         public string? PhoneNumber { get; set; }="";
        public string? NationalId { get; set; }="";


    }

    public class SearchSimilarPersonalInfoQueryHandler : IRequestHandler<SearchSimilarPersonalInfoQuery, object>
    {
        private readonly IPersonalInfoRepository _PersonaInfoRepository;

        public SearchSimilarPersonalInfoQueryHandler(IPersonalInfoRepository PersonaInfoRepository)
        {
            _PersonaInfoRepository = PersonaInfoRepository;
        }
        public async Task<object> Handle(SearchSimilarPersonalInfoQuery request, CancellationToken cancellationToken)
        {
         return    await _PersonaInfoRepository.SearchSimilarPersons(request);
            // return new object{};+

        }
    }
}