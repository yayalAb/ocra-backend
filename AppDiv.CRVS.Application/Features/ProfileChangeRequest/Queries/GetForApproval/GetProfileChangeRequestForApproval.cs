
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.User.Command.Update;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Service.ArchiveService;
using AppDiv.CRVS.Application.Service;

namespace AppDiv.CRVS.Application.Features.ProfileChangeRequests.Query.GetForApproval
{
    // Customer GetProfileChangeForApproval with  response
    public class GetProfileChangeForApproval : IRequest<object>
    {
        public Guid Id { get; set; }

    }

    public class GetProfileChangeForApprovalHandler : IRequestHandler<GetProfileChangeForApproval, object>
    {
        private readonly IProfileChangeRequestRepository _profileChangeRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDateAndAddressService _dateAndAddressService;
        private readonly ILookupFromId _lookupService;
        private readonly IReportRepostory _reportRepostory;
        private readonly IFileService _fileService;

        public GetProfileChangeForApprovalHandler(IProfileChangeRequestRepository profileChangeRequestRepository,
                                                  IUserRepository userRepository,
                                                  IDateAndAddressService dateAndAddressService,
                                                  ILookupFromId lookupService,
                                                  IReportRepostory reportRepostory,
                                                  IFileService fileService)
        {
            _profileChangeRequestRepository = profileChangeRequestRepository;
            _userRepository = userRepository;
            _dateAndAddressService = dateAndAddressService;
            _lookupService = lookupService;
            _reportRepostory = reportRepostory;
            _fileService = fileService;
        }
        public async Task<object> Handle(GetProfileChangeForApproval request, CancellationToken cancellationToken)
        {

            var profileChangeRequest = _profileChangeRequestRepository.GetAll()
                                        .Include(p => p.Request)
                                        .ThenInclude(r => r.Notification)
                                        .ThenInclude(n => n.Sender)
                                        .ThenInclude(s => s.PersonalInfo)
                                        .Where(p => p.Id == request.Id).FirstOrDefault();
            if (profileChangeRequest == null)
            {
                throw new NotFoundException($"profileChangeRequest with id {request.Id}  is not found");
            }
            var content = profileChangeRequest.Content.ToObject<UpdateUserRequest>();
            var mappedPerson = CustomMapper.Mapper.Map<PersonalInfo>(content.PersonalInfo);
            var newData = new {
                content.Id,
                content.PreferedLanguage,
                content.UserImage,
                content.AddressId,
                content.FingerPrintApiUrl,
                PersonalInfo = ReturnPerson.GetPerson(mappedPerson, _dateAndAddressService, _lookupService, _reportRepostory,true),
                // PersonalInfo = CustomMapper.Mapper.Map<UpdatePersonalInfoRequest>(content.PersonalInfo)

            };

            string? userImage;
            try
            {

                var (file, fileName, fileExtenion) = _fileService.getFile(profileChangeRequest.UserId, "UserProfiles", null, null);
                userImage = Convert.ToBase64String(file);
            }
            catch (Exception)
            {
                userImage = null;
            }

            var res = _userRepository.GetAll()
                            .Include(u => u.UserGroups)
                            .Include(u => u.PersonalInfo.ContactInfo)
                            .Where(u => u.Id == profileChangeRequest.UserId)
                            .FirstOrDefault();
            var oldData = new
            {
                res.Id,
                res.PreferedLanguage,
                UserImage = userImage,
                res.AddressId,
                res.FingerPrintApiUrl,
                PersonalInfo = ReturnPerson.GetPerson(res.PersonalInfo, _dateAndAddressService, _lookupService, _reportRepostory),
                // PersonalInfo = CustomMapper.Mapper.Map<UpdatePersonalInfoRequest>(res.PersonalInfo)

            };
            NotificationData? notificationData = null;
            if (profileChangeRequest.Request?.Notification != null)
            {
                notificationData = new NotificationData
                {

                    Message = profileChangeRequest.Request.Notification.MessageStr,
                    ApprovalType = profileChangeRequest.Request.Notification.ApprovalType,
                    SenderId = profileChangeRequest.Request.Notification.SenderId,
                    SenderUserName = profileChangeRequest.Request.Notification.Sender.UserName,
                    SenderFullName = profileChangeRequest.Request.Notification.Sender.PersonalInfo.FirstNameLang + " " +
                                             profileChangeRequest.Request.Notification.Sender.PersonalInfo.MiddleNameLang + " " +
                                             profileChangeRequest.Request.Notification.Sender.PersonalInfo.LastNameLang,
                    Date = (new CustomDateConverter(profileChangeRequest.Request.Notification.CreatedAt)).ethiopianDate,
                };
            }

            return new
            {
                newData,
                oldData,
                profileChangeRequest.Request?.currentStep,
                notificationData,
                RequestId = profileChangeRequest.RequestId

            };
        }
    }
}