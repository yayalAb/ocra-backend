using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;

using MediatR;
using Microsoft.Extensions.Logging;


namespace AppDiv.CRVS.Application.Features.Customers.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetLastGeneratedEventIdByOfficerQuery : IRequest<object>
    {
        public Guid CivilRegOfficerId { get;  set; }

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

            var officer =  _identityService.AllUsersDetail()
            .Where(u => u.PersonalInfo.Id == request.CivilRegOfficerId)
            .FirstOrDefault();

           
            if (officer == null)
            {
                throw new NotFoundException("officer not found");
            }
            var events = _eventRepository.GetAllQueryableAsync();

            var lastEventIdInfo = events
            .Where(e => e.CivilRegOfficer.ApplicationUser.AddressId == officer.AddressId)
                .OrderByDescending(e => e.CertificateId.Substring(e.CertificateId.Length - 4))
                .Select(e => new
                {
                    RegisteredDate = e.EventRegDate,
                    CertificateId = e.CertificateId,
                    LastIdNumber = int.Parse(e.CertificateId.Substring(e.CertificateId.Length -4 )),
                    AddressId = e.CivilRegOfficer.ApplicationUser.AddressId
                }).FirstOrDefault();


            return lastEventIdInfo;
        }
    }
}