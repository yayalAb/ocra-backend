using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.User.Command.Update
{
    public class UpdateUserCommand : IRequest<UpdateUserCommandResponse>
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public bool Status { get; set; }
        public string? Email { get; set; }
        public string PreferedLanguage { get; set; } = "oro";
        public Guid AddressId { get; set; }
        public string? UserImage { get; set; }
        public List<Guid> UserGroups { get; set; }
        public int SelectedAdminType { get; set; }
        public bool? CanRegisterEvent { get; set; } = null;
        public string FingerPrintApiUrl { get; set; } = "localhost";

        [NotMapped]
        public DateTime? WorkStartedOn { get; set; }
        public bool IsFromCommand { get; set; } = false;
        public bool ValidateFirst { get; set; }

        public UpdatePersonalInfoRequest PersonalInfo { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserCommandResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileService _fileService;
        private readonly ILogger<UpdateUserCommandHandler> logger;
        private readonly IWorkHistoryTracker _workHistoryTracker;
        private readonly IBaseRepository<PersonalInfo> personBaseRepo;
        private readonly IAddressLookupRepository _addresslookup;

        public UpdateUserCommandHandler(
            IIdentityService identityService,
            IGroupRepository groupRepository,
            IUserRepository userRepository,
            IFileService fileService,
            ILogger<UpdateUserCommandHandler> logger,
            IWorkHistoryTracker workHistoryTracker,
            IBaseRepository<PersonalInfo> personBaseRepo,
            IAddressLookupRepository addresslookup
            )
        {
            this._fileService = fileService;
            this.logger = logger;
            this._workHistoryTracker = workHistoryTracker;
            this.personBaseRepo = personBaseRepo;
            this._groupRepository = groupRepository;
            _userRepository = userRepository;
            _identityService = identityService;
            _addresslookup = addresslookup;
        }
        public async Task<UpdateUserCommandResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _groupRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = request.IsFromCommand ? null : _groupRepository.Database.BeginTransaction())
                {
                    try
                    {
                        var updateUserCommandRes = new UpdateUserCommandResponse();

                        var validator = new UpdateUserCommandValidator(_userRepository);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);
                        if (validationResult.Errors.Count > 0)
                        {
                            updateUserCommandRes.Success = false;
                            updateUserCommandRes.Status = 400;
                            updateUserCommandRes.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                updateUserCommandRes.ValidationErrors.Add(error.ErrorMessage);
                            updateUserCommandRes.Message = updateUserCommandRes.ValidationErrors[0];
                        }
                        else
                        {
                            if (request.ValidateFirst == true)
                            {
                                updateUserCommandRes.Message = "valid input";
                                updateUserCommandRes.Success = true;
                                return updateUserCommandRes;
                            }
                          
                            await _workHistoryTracker.TrackAsync(request.Id, request.AddressId, request.UserGroups, cancellationToken);
                            // var contact = new ContactInfo
                            // {
                            //     Id = request.PersonalInfo.ContactInfo.Id,
                            //     Email = request.Email,
                            //     Phone = request.PersonalInfo.ContactInfo.Phone,
                            //     HouseNumber = request.PersonalInfo.ContactInfo.HouseNumber,
                            //     Website = request.PersonalInfo.ContactInfo.Website,
                            //     Linkdin = request.PersonalInfo.ContactInfo.Linkdin,
                            //     ModifiedAt = DateTime.Now
                            // };

                            // 2e946713-6676-49ff-8a64-5b43772a6574 group
                            // 15911d9e-2196-47b0-845d-bd99ca25467f addres
                            // 16f8409c-1bbc-4d5e-a530-30aec3076772 lookup

                            var person = new PersonalInfo
                            {
                                Id = request.PersonalInfo.Id,
                                FirstName = request.PersonalInfo.FirstName,
                                MiddleName = request.PersonalInfo.MiddleName,
                                LastName = request.PersonalInfo.LastName,
                                BirthDateEt = request.PersonalInfo.BirthDateEt,
                                NationalId = request.PersonalInfo.NationalId,
                                NationalityLookupId = request.PersonalInfo.NationalityLookupId,
                                SexLookupId = request.PersonalInfo.SexLookupId,
                                PlaceOfBirthLookupId = request.PersonalInfo.PlaceOfBirthLookupId,
                                EducationalStatusLookupId = request.PersonalInfo.EducationalStatusLookupId,
                                TypeOfWorkLookupId = request.PersonalInfo.TypeOfWorkLookupId,
                                MarriageStatusLookupId = request.PersonalInfo?.MarriageStatusLookupId,
                                BirthAddressId = request.PersonalInfo?.BirthAddressId,
                                ResidentAddressId = request.PersonalInfo?.ResidentAddressId,
                                NationLookupId = request.PersonalInfo?.NationLookupId,
                                TitleLookupId = request.PersonalInfo?.TitleLookupId,
                                ReligionLookupId = request.PersonalInfo?.ReligionLookupId,
                                ModifiedAt = DateTime.Now,
                                ContactInfo = CustomMapper.Mapper.Map<ContactInfo>(request.PersonalInfo?.ContactInfo)

                            };
                            var listGroup = await _groupRepository.GetMultipleUserGroups(request.UserGroups);

                            // request.UserGroups.ForEach(async g => listGroup.Add(await _groupRepository.GetByIdAsync(g)));
                            //can use this instead of automapper
                            var user = new ApplicationUser
                            {
                                Id = request.Id,
                                UserName = request.UserName,
                                Email = request.Email,
                                AddressId = request.AddressId,
                                UserGroups = listGroup,
                                PersonalInfo = person,
                                Status = request.Status,
                                PreferedLanguage = request.PreferedLanguage,
                                SelectedAdminType = request.SelectedAdminType,
                                FingerPrintApiUrl = request.FingerPrintApiUrl,
                                CanRegisterEvent = request.CanRegisterEvent
                            };

                            var userAddress = await _addresslookup.GetAsync(user.AddressId);
                            if (userAddress.WorkStartedOn == null)
                            {
                                userAddress.WorkStartedOn = request?.WorkStartedOn;
                                await _addresslookup.UpdateAsync(userAddress, x => x.Id);
                                await _addresslookup.SaveChangesAsync(cancellationToken);
                            }


                            try
                            {
                                await _identityService.UpdateUserAsync(user);

                                if (request.UserImage != null)
                                {

                                    var file = request.UserImage;
                                    var folderName = Path.Combine("Resources", "UserProfiles");
                                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                                    var fileName = request.Id;
                                    await _fileService.UploadBase64FileAsync(file, fileName, pathToSave, FileMode.Create);
                                }
                            }
                            catch (Exception exp)
                            {
                                throw new System.ApplicationException(exp.Message);
                            }

                            var modifiedUser = await _identityService.GetUserByIdAsync(request.Id);

                            // var userResponse = CustomMapper.Mapper.Map<UserResponseDTO>(modifiedUser);
                            var userResponse = new UserResponseDTO
                            {

                            };
                            if (transaction != null)
                            {
                                await transaction.CommitAsync();
                            }
                            var personEntries = new List<PersonalInfoEntry>{
                            new PersonalInfoEntry{
                                PersonalInfoId = user.PersonalInfo.Id,
                                State = EntityState.Modified
                                }
                        };
                            personBaseRepo.TriggerPersonalInfoIndex(personEntries);
                        }
                            return updateUserCommandRes;
                    }
                    catch (Exception ex)
                    {
                        if (transaction != null)
                        {
                            await transaction.RollbackAsync();
                        }
                        throw new System.ApplicationException(ex.Message);
                    }
                }
            });
        }
    }
}