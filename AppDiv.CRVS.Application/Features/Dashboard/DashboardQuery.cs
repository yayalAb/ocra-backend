using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
using MediatR;


namespace AppDiv.CRVS.Application.Features.Dashboard

{
    // Customer query with List<Customer> response
    public record DashboardQuery : IRequest<object>
    {

        public string StartDate { set; get; }
        public string EndDate { get; set; }

    }

    public class DashboardQueryHandler : IRequestHandler<DashboardQuery, object>
    {
        private readonly IEventRepository _eventRepository;
        private readonly CustomDateConverter _dateConverter;

        public DashboardQueryHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _dateConverter = new CustomDateConverter();
        }
        public async Task<object> Handle(DashboardQuery request, CancellationToken cancellationToken)
        {

            DateTime startDate = _dateConverter.EthiopicToGregorian(request.StartDate);
            DateTime EndDate = _dateConverter.EthiopicToGregorian(request.EndDate);
            var lookuplist = _eventRepository.GetAll().Where(ev =>
             ev.EventRegDate >= startDate &&
             ev.EventRegDate <= EndDate).GroupBy(x => x.EventType).Select(x =>
                            new
                            {
                                Event = x.Key,
                                Count = x.Count()
                            }

                        );
            return lookuplist; // object
            ;
        }
    }
}