using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using MediatR;

namespace AppDiv.CRVS.Application.Features.User.Command.Update
{
    public class UpdateUserCommand : IRequest<UserResponseDTO>
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public AddPersonalInfoRequest PersonalInfo { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponseDTO>
    {
        private readonly IIdentityService _identityService;
        public UpdateUserCommandHandler(IIdentityService identityService)
        {
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
                Linkdin = request.PersonalInfo.ContactInfo.Linkdin
            };

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
                ContactInfo = contact

            };

            //can use this instead of automapper
            var user = new ApplicationUser
            {
                Id = request.Id,
                UserName = request.UserName,
                Email = request.Email,
                PersonalInfo = person,

            };

            try
            {
                await _identityService.UpdateUserAsync(user);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedUser = await _identityService.GetUserByIdAsync(request.Id);

            var userResponse = CustomMapper.Mapper.Map<UserResponseDTO>(modifiedUser);
            return userResponse;
        }
    }
}