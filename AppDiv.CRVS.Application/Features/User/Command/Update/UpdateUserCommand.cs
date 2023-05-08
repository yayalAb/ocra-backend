using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.User.Command.Update
{
    public class UpdateUserCommand : IRequest<UserResponseDTO>
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? UserImage { get; set; }
        public List<Guid> UserGroups { get; set; }
        public AddPersonalInfoRequest PersonalInfo { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponseDTO>
    {
        private readonly IIdentityService _identityService;
        private readonly IGroupRepository _groupRepository;
        private readonly IFileService _fileService;
        private readonly ILogger<UpdateUserCommandHandler> logger;

        public UpdateUserCommandHandler(IIdentityService identityService, IGroupRepository groupRepository, IFileService fileService , ILogger<UpdateUserCommandHandler> logger)
        {
            this._fileService = fileService;
            this.logger = logger;
            this._groupRepository = groupRepository;
            _identityService = identityService;
        }
        public async Task<UserResponseDTO> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var contact = new ContactInfo
            {
                Id = request.PersonalInfo.ContactInfo.Id,
                Email = request.Email,
                Phone = request.PersonalInfo.ContactInfo.Phone,
                HouseNumber = request.PersonalInfo.ContactInfo.HouseNo,
                Website = request.PersonalInfo.ContactInfo.Website,
                Linkdin = request.PersonalInfo.ContactInfo.Linkdin,
                ModifiedAt = DateTime.Now
            };
            // 2e946713-6676-49ff-8a64-5b43772a6574 group
            // 15911d9e-2196-47b0-845d-bd99ca25467f addres
            // 16f8409c-1bbc-4d5e-a530-30aec3076772 lookup

            var person = new PersonalInfo
            {
                Id = request.PersonalInfo.Id,
                FirstName = request.PersonalInfo.FirstName,
                MiddleName = request.PersonalInfo.MiddleName,
                LastName = request.PersonalInfo.LastName,
                BirthDate = request.PersonalInfo.BirthDate,
                NationalId = request.PersonalInfo.NationalId,
                NationalityLookupId = request.PersonalInfo.NationalityLookupId,
                SexLookupId = request.PersonalInfo.SexLookupId,
                PlaceOfBirthLookupId = request.PersonalInfo.PlaceOfBirthLookupId,
                EducationalStatusLookupId = request.PersonalInfo.EducationalStatusLookupId,
                TypeOfWorkLookupId = request.PersonalInfo.TypeOfWorkLookupId,
                MarriageStatusLookupId = request.PersonalInfo.MarriageStatusLookupId,
                AddressId = request.PersonalInfo.AddressId,
                NationLookupId = request.PersonalInfo.NationLookupId,
                TitleLookupId = request.PersonalInfo.TitleLookupId,
                ReligionLookupId = request.PersonalInfo.ReligionLookupId,
                ModifiedAt = DateTime.Now,
                ContactInfo = contact

            };
            var listGroup = await _groupRepository.GetMultipleUserGroups(request.UserGroups);

            // request.UserGroups.ForEach(async g => listGroup.Add(await _groupRepository.GetByIdAsync(g)));
            //can use this instead of automapper
            var user = new ApplicationUser
            {
                Id = request.Id,
                UserName = request.UserName,
                Email = request.Email,
                UserGroups = listGroup,
                PersonalInfo = person

            };

            try
            {
                await _identityService.UpdateUserAsync(user);

                var file = request.UserImage;
                var folderName = Path.Combine("Resources", "UserProfiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fileName = request.Id;
                logger.LogCritical(file);

                await _fileService.UploadBase64FileAsync(file, fileName, pathToSave, FileMode.Create);
            }
            catch (Exception exp)
            {
                throw new System.ApplicationException(exp.Message);
            }

            var modifiedUser = await _identityService.GetUserByIdAsync(request.Id);

            // var userResponse = CustomMapper.Mapper.Map<UserResponseDTO>(modifiedUser);
            var userResponse = new UserResponseDTO{

            };
            return userResponse;
        }
    }
}