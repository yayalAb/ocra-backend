using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.Search
{
    // Customer SearchCertificateQuery with  response
    public class SearchCertificateQuery : IRequest<object>
    {
        public string SearchString { get; set; }

    }

    public class SearchCertificateQueryHandler : IRequestHandler<SearchCertificateQuery, object>
    {
        private readonly ICertificateRepository _CertificateRepository;

        public SearchCertificateQueryHandler(ICertificateRepository CertificateRepository)
        {
            _CertificateRepository = CertificateRepository;
        }
        public async Task<object> Handle(SearchCertificateQuery request, CancellationToken cancellationToken)
        {
            // var SelectedInfo = _CertificateRepository.GetAll().Where(model =>
            //                                     EF.Functions.Like(model.CertificateSerialNumber, $"%{request.SearchString}%")
            //                                 || EF.Functions.Like(model.ContentStr, $"%{request.SearchString}%"))
            //                                         .Select(an => new SearchCertificateResponseDTO
            //                                         {
            //                                             Id = an.Id,
            //                                             CertifceteType = an.Event.EventType,
            //                                             FullName = an.Event.EventOwener.FirstNameLang + " " + an.Event.EventOwener.MiddleNameLang + " " + an.Event.EventOwener.LastNameLang
            //                                         }).Take(50);

            // return SelectedInfo;
            return await _CertificateRepository.SearchCertificate(request);
        }
    }
}