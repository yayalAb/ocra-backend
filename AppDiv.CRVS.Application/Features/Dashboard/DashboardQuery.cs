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
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICorrectionRequestRepostory _correctionrequest;
        

        public DashboardQueryHandler(ICorrectionRequestRepostory correctionrequest,ICertificateRepository certificateRepository,IEventRepository eventRepository,IAddressLookupRepository userRpo, IUserResolverService userResolverService,ILookupRepository LookupsRepo)
        {
            _eventRepository = eventRepository;
            _dateConverter = new CustomDateConverter();
            _userResolverService=userResolverService;
            _LookupsRepo=LookupsRepo;
            _userRpo=userRpo;
            _certificateRepository=certificateRepository;
            _correctionrequest = correctionrequest;
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
            .Where(x=>x.EventRegDate >= startDate && x.EventRegDate <= EndDate)
            .Where(x=>x.EventRegisteredAddressId==AddressId||x.EventRegisteredAddress.ParentAddressId==AddressId
            ||x.EventRegisteredAddress.ParentAddress.ParentAddressId==AddressId||x.EventRegisteredAddress.ParentAddress.ParentAddress.ParentAddressId==AddressId
            ||x.EventRegisteredAddress.ParentAddress.ParentAddress.ParentAddress.ParentAddressId==AddressId);
            
            var certificates = _certificateRepository.GetAll()
                                .Include(x=>x.Event)
                                .ThenInclude(x=>x.EventRegisteredAddress)
                                .ThenInclude(x=>x.ParentAddress)
                                .ThenInclude(x=>x.ParentAddress)
                                .ThenInclude(x=>x.ParentAddress)
                                .ThenInclude(x=>x.ParentAddress)
                                .Where(x=>(x.Status==true && x.AuthenticationStatus==true)&&(x.AuthenticationAt >= startDate && x.AuthenticationAt <= EndDate)&&
                                 x.Event.EventRegisteredAddressId==AddressId||x.Event.EventRegisteredAddress.ParentAddressId==AddressId
                                ||x.Event.EventRegisteredAddress.ParentAddress.ParentAddressId==AddressId||x.Event.EventRegisteredAddress.ParentAddress.ParentAddress.ParentAddressId==AddressId
                                ||x.Event.EventRegisteredAddress.ParentAddress.ParentAddress.ParentAddress.ParentAddressId==AddressId);
                                
            var CorrectionRequests= _correctionrequest.GetAll()
            .Include(x=>x.Request)
            .ThenInclude(x=>x.CivilRegOfficer)
            .ThenInclude(x=>x.ApplicationUser)
            .ThenInclude(x=>x.Address)
            .ThenInclude(x=>x.ParentAddress)
            .ThenInclude(x=>x.ParentAddress)
            .ThenInclude(x=>x.ParentAddress)
            .ThenInclude(x=>x.ParentAddress)
            .Where(x=>(x.CreatedAt >= startDate && x.CreatedAt <= EndDate)&&
                                 x.Request.CivilRegOfficer.ApplicationUser.AddressId==AddressId||x.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddressId==AddressId
                                ||x.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddressId==AddressId||x.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.ParentAddressId==AddressId
                                ||x.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.ParentAddress.ParentAddressId==AddressId);
            var Approval=new {
                Authentication = certificates.Count(),
                Verification = allEvents.Count(e => e.IsVerified),
                Change = CorrectionRequests.Count()
            };
            var groupedEvent= allEvents
                .GroupBy(x => x.EventType);
            var EventReport = groupedEvent.Select(x => new
                {
                    Event = x.Key,
                    Count = x.Count()
                });
            
            var EventPivotReport = allEvents.Select(x =>  new DashbordResponse
                {
                   Event = x.EventType,
                   Active=x.Status.ToLower()=="active"? 1:0,
                   Delay=x.Status.ToLower()!="active"? 1:0,
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
    }
    public class DashbordResponse{

           public string? Event {get; set;}
           public int? Active {get; set;}
           public int? Delay {get; set;}
           public string? Gender {get; set;}
           public string? Address {get; set;}
    }

}

       