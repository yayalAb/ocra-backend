using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnVerficationList : IReturnVerficationList
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserResolverService _userResolverService;
        public ReturnVerficationList(IEventRepository eventRepository, IUserRepository userRepository, IUserResolverService userResolverService)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _userResolverService = userResolverService;
        }
        async Task<IQueryable<Event>> IReturnVerficationList.GetVerficationRequestedCertificateList(bool isVerfication=true)
        {
            var applicationuser = _userRepository.GetAll()
           .Include(x => x.Address)
           .Include(x => x.UserGroups)
           .Where(x => x.PersonalInfoId == _userResolverService.GetUserPersonalId()).FirstOrDefault();
            if (applicationuser == null)
            {
                throw new NotFoundException("user does not exist");
            }
            Guid userGroupId = applicationuser.UserGroups.Select(x => x.Id).FirstOrDefault();
            IQueryable<Event> eventsQueriable;
            eventsQueriable = _eventRepository.GetAllQueryableAsync()
               .Include(x => x.CivilRegOfficer)
               .ThenInclude(x => x.ApplicationUser)
               .ThenInclude(a => a.Address)
               .ThenInclude(p => p.ParentAddress)
               .ThenInclude(p => p.ParentAddress)
               .ThenInclude(p => p.ParentAddress)
               .Include(x=>x.EventCertificates)
               .Include(x => x.VerficationRequestNavigation)
               .ThenInclude(x => x.Request)
               .ThenInclude(x => x.Workflow)
               .ThenInclude(s => s.Steps);

             if(isVerfication){
              eventsQueriable=eventsQueriable.Where(e => (e.IsCertified && !e.IsVerified) && ((e.VerficationRequestNavigation != null)
               && (e.VerficationRequestNavigation.Request.Workflow.Steps.FirstOrDefault() != null))
              && (e.VerficationRequestNavigation.Request.Workflow.Steps
              .Where(s => s.step == e.VerficationRequestNavigation.Request.NextStep && s.UserGroupId == userGroupId).FirstOrDefault() != null
               ));
             }
             else{
            eventsQueriable= eventsQueriable.Where(e => e.EventCertificates
            .Where(s=>s.Status && s.AuthenticationAt<DateTime.Now.AddDays(30)).FirstOrDefault().AuthenticationStatus);
             }  

            if (applicationuser.Address.AdminLevel == 1)
            {
                eventsQueriable = eventsQueriable.Where(e => (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.ParentAddress.Id == applicationuser.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.Id == applicationuser.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == applicationuser.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.Id == applicationuser.AddressId));
            }
            else if (applicationuser.Address.AdminLevel == 2)
            {
                eventsQueriable = eventsQueriable.Where(e => (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.ParentAddress.Id == applicationuser.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.Id == applicationuser.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == applicationuser.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.Id == applicationuser.AddressId));
            }
            else if (applicationuser.Address.AdminLevel == 3)
            {
                eventsQueriable = eventsQueriable.Where(e => (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.Id == applicationuser.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == applicationuser.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.Id == applicationuser.AddressId));
            }
            else if (applicationuser.Address.AdminLevel == 4)
            {
                eventsQueriable = eventsQueriable.Where(e => (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == applicationuser.AddressId)
                || (e.CivilRegOfficer.ApplicationUser.Address.Id == applicationuser.AddressId));
            }
            else if (applicationuser.Address.AdminLevel == 5)
            {
                eventsQueriable = eventsQueriable.Where(e => (e.CivilRegOfficer.ApplicationUser.Address.Id == applicationuser.AddressId));
            }
            return eventsQueriable;
        }

    }
}

