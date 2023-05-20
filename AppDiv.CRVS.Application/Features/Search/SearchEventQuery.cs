
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.Search
{
    // Customer SearchEventQuery with  response
    public class SearchEventQuery : IRequest<object>
    {
        public string SearchString { get; set; }

    }

    public class SearchEventQueryHandler : IRequestHandler<SearchEventQuery, object>
    {
        private readonly IEventRepository _eventRepository;

        public SearchEventQueryHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task<object> Handle(SearchEventQuery request, CancellationToken cancellationToken)
        {
            var SelectedInfo = _eventRepository.GetAll().Where(model =>
                                                EF.Functions.Like(model.EventType, $"%{request.SearchString}%")
                                              || EF.Functions.Like(model.RegBookNo, $"%{request.SearchString}%")
                                               || EF.Functions.Like(model.CertificateId, $"%{request.SearchString}%")
                                                || EF.Functions.Like(model.EventOwener.FirstNameStr, $"%{request.SearchString}%")
                                                 || EF.Functions.Like(model.EventOwener.MiddleNameStr, $"%{request.SearchString}%")
                                                  || EF.Functions.Like(model.EventOwener.LastNameStr, $"%{request.SearchString}%")
                                                  || EF.Functions.Like(model.CivilRegOfficer.LastNameStr, $"%{request.SearchString}%")
                                                  || EF.Functions.Like(model.CivilRegOfficer.LastNameStr, $"%{request.SearchString}%")
                                                  || EF.Functions.Like(model.CivilRegOfficer.LastNameStr, $"%{request.SearchString}%")


                                               )
                                                    .Select(an => new SearchEventResponseDTO
                                                    {
                                                        Id = an.Id,
                                                        CertifceteType = an.EventType,
                                                        FullName = an.EventOwener.FirstNameLang + " " + an.EventOwener.MiddleNameLang + " " + an.EventOwener.LastNameLang,
                                                        CertifceteId = an.CertificateId,
                                                        booknumber = an.RegBookNo,
                                                        CivilRegOfficer = an.CivilRegOfficer.FirstNameLang + " " + an.CivilRegOfficer.MiddleNameLang + " " + an.CivilRegOfficer.LastNameLang,

                                                    }).Take(50);

            return SelectedInfo;
        }
    }
}