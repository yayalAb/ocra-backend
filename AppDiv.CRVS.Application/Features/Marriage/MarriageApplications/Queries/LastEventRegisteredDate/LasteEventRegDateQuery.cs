using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Utility.Services;


namespace AppDiv.CRVS.Application.Features.Marriage.MarriageApplications.Queries.LastEventRegisteredDate
{
    // Customer GetCustomerByIdQuery with Customer response
    public class LasteEventRegDateQuery : IRequest<object>
    {
        public Guid CivilRegOfficerId { get; set; }

    }

    public class LasteEventRegDateQueryHandler : IRequestHandler<LasteEventRegDateQuery, object>
    {
        private readonly IEventRepository _eventRepository;

        public LasteEventRegDateQueryHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task<object> Handle(LasteEventRegDateQuery request, CancellationToken cancellationToken)
        {
            var convertor = new CustomDateConverter();
            var LastRegEvent = _eventRepository.GetAll()
            .Where(x => x.CivilRegOfficerId == request.CivilRegOfficerId).OrderByDescending(x => x.CreatedAt).FirstOrDefault();

            return new
            {
                lastdate = LastRegEvent?.CreatedAt
            };

        }
    }

}