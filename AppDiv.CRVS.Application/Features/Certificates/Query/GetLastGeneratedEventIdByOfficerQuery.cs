using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Features.Customers.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetLastGeneratedEventIdByOfficerQuery : IRequest<object>
    {
        public Guid CivilRegOfficerId { get; set; }
        public string? year { get; set; }

    }

    public class GetLastGeneratedEventIdByOfficerQueryHandler : IRequestHandler<GetLastGeneratedEventIdByOfficerQuery, object>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IIdentityService _identityService;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly ILogger<GetLastGeneratedEventIdByOfficerQueryHandler> logger;

        public GetLastGeneratedEventIdByOfficerQueryHandler(IEventRepository eventRepository, IIdentityService identityService,
        IPersonalInfoRepository personalInfoRepository, ILogger<GetLastGeneratedEventIdByOfficerQueryHandler> logger)
        {
            _eventRepository = eventRepository;
            _identityService = identityService;
            _personalInfoRepository = personalInfoRepository;
            this.logger = logger;
        }
        public async Task<object> Handle(GetLastGeneratedEventIdByOfficerQuery request, CancellationToken cancellationToken)
        {


            var officer = _identityService.AllUsersDetail()
            .Where(u => u.PersonalInfoId == request.CivilRegOfficerId)
            .Include(e => e.Address)
            .FirstOrDefault();


            if (officer == null)
            {
                throw new NotFoundException("officer not found");
            }
            try
            {
                request.year = string.IsNullOrEmpty(request.year) ? new CustomDateConverter(DateTime.Now).ethiopianDate : request.year;
                DateTime SentYear = new CustomDateConverter(request.year).gorgorianDate;
                DateTime nowYear = DateTime.Now;
                DateTime year = (string.IsNullOrEmpty(request.year) || SentYear.Year == nowYear.Year) ? nowYear : nowYear.AddYears(-1);
                var ethiopiandate = new CustomDateConverter(year).ethiopianDate;
                var lastEventIdInfo = _eventRepository.GetAllQueryableAsync()
                .Where(e => e.CivilRegOfficer.ApplicationUser.AddressId == officer.AddressId && e.EventRegDate.Year == year.Year)
                    .OrderByDescending(e => e.CertificateId.Substring(e.CertificateId.Length - 4)).FirstOrDefault();
                if (lastEventIdInfo == null)
                {
                    return new
                    {
                        LastIdNumber = 0000,
                        AddressCode = officer?.Address?.Code,
                        year = ethiopiandate
                    };

                }
                else
                {
                    return new
                    {
                        LastIdNumber = int.Parse(lastEventIdInfo?.CertificateId?.Substring(lastEventIdInfo.CertificateId.Length - 4)),
                        AddressCode = officer?.Address?.Code,
                        year = ethiopiandate
                    };
                }
            }
            catch (Exception)
            {
                throw new NotFoundException("please Check the dat format, the date format must be dd/mm/yyyy");
            }
        }
    }
}