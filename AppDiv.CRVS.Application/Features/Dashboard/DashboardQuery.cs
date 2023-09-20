using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
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

        public string? StartDate { set; get; }
        public string? EndDate { get; set; }
        public Guid? AddressId { get; set; }

    }

    public class DashboardQueryHandler : IRequestHandler<DashboardQuery, object>
    {
        private readonly IEventRepository _eventRepository;
        private readonly CustomDateConverter _dateConverter;
        private readonly IUserResolverService _userResolverService;
        private readonly ILookupRepository _LookupsRepo;
        private readonly IAddressLookupRepository _userRpo;
        private readonly ISettingRepository _SettinglookupRepository;
        private static int BirthSetting; 
       private static int AdoptionSetting;
       private static int MarriageSetting;
       private static int DivorceSetting;
       private static int DeathSetting;
        


        public DashboardQueryHandler(ISettingRepository SettinglookupRepository,IEventRepository eventRepository,IAddressLookupRepository userRpo, IUserResolverService userResolverService,ILookupRepository LookupsRepo)
        {
            _eventRepository = eventRepository;
            _dateConverter = new CustomDateConverter();
            _userResolverService=userResolverService;
            _LookupsRepo=LookupsRepo;
            _userRpo=userRpo;
            _SettinglookupRepository=SettinglookupRepository;
        }
        public async Task<object> Handle(DashboardQuery request, CancellationToken cancellationToken)
        {
            DateTime startDate=DateTime.Now.AddMonths(-1);
            DateTime EndDate=DateTime.Now;
            if(!(string.IsNullOrEmpty(request.StartDate)||string.IsNullOrEmpty(request.StartDate))){
                    startDate = _dateConverter.EthiopicToGregorian(request.StartDate);
                    EndDate = _dateConverter.EthiopicToGregorian(request.EndDate);
            }
            Guid AddressId=_userResolverService.GetWorkingAddressId();
            if(request.AddressId!=null &&request.AddressId!=Guid.Empty){
               AddressId=(Guid)request.AddressId;
            }
            var setting=_SettinglookupRepository.GetAll();
            BirthSetting =   int.Parse(setting.Where(x => x.Key == "birthSetting").FirstOrDefault()!.Value.Value<string>("active_registration")!);
            AdoptionSetting = int.Parse(setting.Where(x => x.Key == "adoptionSetting").FirstOrDefault()!.Value.Value<string>("active_registration")!);
            MarriageSetting = int.Parse(setting.Where(x => x.Key == "marriageSetting").FirstOrDefault()!.Value.Value<string>("active_registration")!);
            DivorceSetting = int.Parse(setting.Where(x => x.Key == "divorceSetting").FirstOrDefault()!.Value.Value<string>("active_registration")!);
            DeathSetting =  int.Parse(setting.Where(x => x.Key == "deathSetting").FirstOrDefault()!.Value.Value<string>("active_registration")!);
            
            var CardValue=new ReportCardResponsDTO();
            Guid MalelookupId = _LookupsRepo.GetAll().Where(l => l.Key == "sex")
                                                .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                                                    || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                                                    || EF.Functions.Like(l.ValueStr, "Male%"))
                                                .Select(l => l.Id).FirstOrDefault();

            Guid FemalelookupId  = _LookupsRepo.GetAll().Where(l => l.Key == "sex")
                                                    .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                                                        || EF.Functions.Like(l.ValueStr, "%Dubara%")
                                                        || EF.Functions.Like(l.ValueStr, "%Female%"))
                                                    .Select(l => l.Id).FirstOrDefault();
            var allEvents = _eventRepository.GetAll()
            .Include(x=>x.EventOwener)
            .Include(x=>x.EventOwener.SexLookup)
            .Include(x=>x.EventRegisteredAddress)
            .ThenInclude(x=>x.ParentAddress)
            .ThenInclude(x=>x.ParentAddress)
            .ThenInclude(x=>x.ParentAddress)
            .ThenInclude(x=>x.ParentAddress)
            .Include(x=>x.EventCertificates)
            .Include(x=>x.CorrectionRequests)
            .ThenInclude(x=>x.Request)
            .Where(x=>(x.EventRegDate >= startDate && x.EventRegDate <= EndDate)
            // ||x.CorrectionRequests.Any(c=>c.CreatedAt>= startDate && c.CreatedAt <= EndDate)
            // ||x.EventCertificates.Any(c=>c.AuthenticationAt>= startDate && c.AuthenticationAt <= EndDate)
            )
            .Where(x=>x.EventRegisteredAddressId==AddressId||x.EventRegisteredAddress.ParentAddressId==AddressId
            ||x.EventRegisteredAddress.ParentAddress.ParentAddressId==AddressId||x.EventRegisteredAddress.ParentAddress.ParentAddress.ParentAddressId==AddressId
            ||x.EventRegisteredAddress.ParentAddress.ParentAddress.ParentAddress.ParentAddressId==AddressId);
            var Approval=new {
                Authentication = allEvents.Count(e => e.EventCertificates.Where(x=>x.Status==true).FirstOrDefault().AuthenticationStatus==true),
                Verfication = allEvents.Count(e => e.IsVerified),
                Change = allEvents.Sum(e => e.CorrectionRequests.Count(x => x.Request.NextStep == x.Request.currentStep))
            };
            var groupedEvent= allEvents
                .GroupBy(x => x.EventType);
            var EventReport = groupedEvent.Select(x => new
                {
                    Event = x.Key,
                    Count = x.Count()
                });
            
            var EventPivotReport = allEvents.Select(x => new
                {
                   Event = x.EventType,
                   status=ReturneventStatus(x.EventType, x.EventDate,x.EventRegDate),
                   Gender=x.EventOwener.SexLookup.ValueLang,
                   Address=x.EventRegisteredAddressId==AddressId||x.EventRegisteredAddress.ParentAddressId==AddressId?x.EventRegisteredAddress.AddressNameLang:
                   x.EventRegisteredAddress.ParentAddress.ParentAddressId==AddressId? x.EventRegisteredAddress.ParentAddress.AddressNameLang:
                   x.EventRegisteredAddress.ParentAddress.ParentAddress.ParentAddressId==AddressId? x.EventRegisteredAddress.ParentAddress.ParentAddress.AddressNameLang:""
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
                ApprovalResponse=Approval,
                EventPivotResponse=EventPivotReport
            }; 
        }
   public static  string ReturneventStatus(string EventType, DateTime eventDate, DateTime EventRegDate){
        int days=0;
            switch(EventType.ToLower()){
               case "birth":
                     days=BirthSetting;
                     break; 
                case "death":
                     days=DeathSetting;
                     break;
                case "divorce":
                     days=DivorceSetting;
                     break;
                case "adoption":
                     days=AdoptionSetting;
                     break;
                case "marriage":
                     days=MarriageSetting;
                     break;
            }
            TimeSpan deff = EventRegDate - eventDate;
            int daysDef = Convert.ToInt32(deff.TotalDays);
           return daysDef <= days ?"Active":"Delay";
    }
    }
}
       