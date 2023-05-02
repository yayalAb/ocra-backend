using System.Net.Cache;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Config;
using AppDiv.CRVS.Utility.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace AppDiv.CRVS.Application.Features.User.Command.Create
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IGroupRepository _groupRepository;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;
        private readonly SMTPServerConfiguration _config;

        public CreateUserCommandHandler(IIdentityService identityService,
                                        IGroupRepository groupRepository,
                                        IFileService fileService, IMailService mailService,
                                        IOptions<SMTPServerConfiguration> config)
        {
            this._groupRepository = groupRepository;
            _identityService = identityService;
            _fileService = fileService;
            _mailService = mailService;
            _config = config.Value;
        }
        public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {


            var CreateUserCommadResponse = new CreateUserCommandResponse();

            var validator = new CreateUserCommandValidator(_identityService);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                CreateUserCommadResponse.Success = false;
                CreateUserCommadResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateUserCommadResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateUserCommadResponse.Message = CreateUserCommadResponse.ValidationErrors[0];
            }
            if (CreateUserCommadResponse.Success)
            {
                var contact = new ContactInfo
                {
                    Id = request.PersonalInfo.ContactInfo.Id,
                    Email = request.Email,
                    Phone = request.PersonalInfo.ContactInfo.Phone,
                    HouseNumber = request.PersonalInfo.ContactInfo.HouseNo,
                    Website = request.PersonalInfo.ContactInfo.Website,
                    Linkdin = request.PersonalInfo.ContactInfo.Linkdin,
                    CreatedAt = DateTime.Now
                };
                // request.userImage
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
                    ContactInfo = contact,
                    CreatedAt = DateTime.Now

                };
                var listGroup = await _groupRepository.GetMultipleUserGroups(request.UserGroups);

                // request.UserGroups.ForEach(async g => listGroup.Add(await _groupRepository.GetAsync(g)));
                //can use this instead of automapper
                listGroup = await _groupRepository.GetMultipleUserGroups(request.UserGroups);
                var user = new ApplicationUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    UserGroups = listGroup,
                    PersonalInfo = person,

                };
                var response = await _identityService.createUser(user);


                // save profile image
                var file = request.UserImage;
                var folderName = Path.Combine("Resources", "UserProfiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fileName = response.id;
                _fileService.UploadBase64File(file, fileName, pathToSave, FileMode.Create);

                //send password by email    
                var emailContent = response.password + "  is your default password you can login and change it";
                var subject = "Welcome to OCRVS";
                await _mailService.SendAsync(body: emailContent, subject: subject, senderMailAddress: _config.SENDER_ADDRESS, receiver: user.Email, cancellationToken);
            }
            return CreateUserCommadResponse;
        }
    }
}
