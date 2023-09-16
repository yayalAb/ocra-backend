using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.Dashboard

{
    // Customer query with List<Customer> response
    public record DashboardQuery : IRequest<object>
    {

        public string StartDate { set; get; }
        public string EndDate { get; set; }
        public Guid? AddressId { get; set; }

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
            var CardValue=new ReportCardResponsDTO();
            Console.WriteLine("startDate {0}, EndDate{1} ",startDate,EndDate);
            // .Where(ev =>
            //  ev.EventRegDate >= startDate &&
            //  ev.EventRegDate <= EndDate)
            var allEvents = _eventRepository.GetAll()
            .Include(x=>x.EventCertificates);
            var Approval=new {
                Authentication = allEvents.Count(e => e.EventCertificates.Where(x=>x.Status==true).FirstOrDefault().AuthenticationStatus==true),
                Verfication = allEvents.Count(e => e.IsVerified),
                Change = allEvents.Count(e => e.IsVerified)
            };
            

            var EventReport = allEvents
                .GroupBy(x => x.EventType)
                .Select(x => new
                {
                    Event = x.Key,
                    Count = x.Count()
                });
               foreach(var events in  EventReport){
                  switch(events.Event){
                    case "Birth":
                        CardValue.Birth=events.Count;
                        break;
                    case "Death":
                        CardValue.Death=events.Count;
                        break;  
                    case "Divorce":
                        CardValue.Divorce=events.Count;
                        break;
                    case "Adoption":
                        CardValue.Adoption=events.Count;
                        break;  
                    case "Marriage":
                        CardValue.Marriage=events.Count;
                        break;        
                  }
               }
                     
            return new{
                EventResponse=CardValue,
                ApprovalResponse=Approval
            }; 
        }
    }
}
       