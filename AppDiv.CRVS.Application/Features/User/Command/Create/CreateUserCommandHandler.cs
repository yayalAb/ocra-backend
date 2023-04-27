using System.Net.Cache;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;

namespace AppDiv.CRVS.Application.Features.User.Command.Create
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IGroupRepository _groupRepository;
        private readonly IFileService _fileService;
        public CreateUserCommandHandler(IIdentityService identityService,
                                        IGroupRepository groupRepository,
                                        IFileService fileService)
        {
            this._groupRepository = groupRepository;
            _identityService = identityService;
            _fileService = fileService;

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
                    MarriageStatusLookupId = request.PersonalInfo.MarriageStatusId,
                    AddressId = request.PersonalInfo.AddressId,
                    NationLookupId = request.PersonalInfo.NationLookupId,
                    TitleLookupId = request.PersonalInfo.TitleLookupId,
                    ReligionLookupId = request.PersonalInfo.ReligionId,
                    ContactInfo = contact,
                    CreatedAt = DateTime.Now

                };
                var listGroup = new List<UserGroup>();
                request.UserGroups.ForEach(async g => listGroup.Add(await _groupRepository.GetByIdAsync(g)));
                //can use this instead of automapper
                var user = new ApplicationUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    UserGroups = listGroup,
                    PersonalInfo = person,

                };
                var response = await _identityService.createUser(user);

                var file = request.userImage;
                var folderName = Path.Combine("Resources", "UserProfiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fileName = response.id;

                _fileService.UploadBase64File(file, fileName, pathToSave, FileMode.Create);

            }
            return CreateUserCommadResponse;
        }
    }
}
