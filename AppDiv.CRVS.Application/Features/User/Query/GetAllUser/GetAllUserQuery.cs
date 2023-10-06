
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Query.GetAllUser

{
    // Customer query with List<Customer> response
    public record GetAllUserQuery : IRequest<PaginatedList<UserResponseDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
    }

    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, PaginatedList<UserResponseDTO>>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserResolverService _UserResolver;
        private readonly IDateAndAddressService _addressService;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly CustomDateConverter _convertor = new();
        // private readonly IMapper _mapper;

        public GetAllUserQueryHandler(
            IHttpContextAccessor httpContext,
            IUserRepository userRepository,
            IIdentityService identityService,
            IUserResolverService UserResolver,
            IDateAndAddressService addressService
            )
        {
            _identityService = identityService;
            _UserResolver = UserResolver;
            this._addressService = addressService;
            _userRepository = userRepository;
            _httpContext = httpContext;
            // _mapper = mapper;
        }
        public async Task<PaginatedList<UserResponseDTO>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            Guid userId = _UserResolver.GetUserPersonalId();
            if (userId == null || userId == Guid.Empty)
            {
                throw new NotFoundException("User Does not Found");
            }
            var response = _userRepository
            .GetAll().OrderByDescending(e => e.CreatedAt)
            .Include(x => x.Address)
            .Include(x => x.PersonalInfo)
            .Where(x => x.PersonalInfoId == userId).FirstOrDefault();
            if (response == null)
            {
                throw new NotFoundException("User Does not Found, Please Logout And Login");
            }
            var response2 = _userRepository
                               .GetAll().OrderByDescending(e => e.CreatedAt)
                               .Include(x => x.Address)
                                   .ThenInclude(x => x.ParentAddress)
                                       .ThenInclude(x => x.ParentAddress)
                                           .ThenInclude(x => x.ParentAddress).AsQueryable();
            if (response2 == null)
            {
                throw new NotFoundException("invalid User Address");
            }
            if (response?.Address?.AdminLevel == 1)
            {
                response2 = response2.Where(x => ((
                             x.Address.ParentAddress.ParentAddress.ParentAddress.ParentAddress.Id == response.AddressId ||
                            (x.Address.ParentAddress.ParentAddress.ParentAddress.Id == response.AddressId
                             || x.Address.ParentAddress.ParentAddress.Id == response.AddressId)
                             || (x.Address.ParentAddressId == response.AddressId
                             || x.Address.Id == response.AddressId)))
                             );
            }
            else if (response?.Address?.AdminLevel == 2)
            {
                response2 = response2.Where(x => ((
                            (x.Address.ParentAddress.ParentAddress.ParentAddress.Id == response.AddressId
                             || x.Address.ParentAddress.ParentAddress.Id == response.AddressId)
                             || (x.Address.ParentAddressId == response.AddressId
                             || x.Address.Id == response.AddressId)))
                             );
            }
            else if (response?.Address?.AdminLevel == 3)
            {
                response2 = response2.Where(x => (
                            (x.Address.ParentAddress.ParentAddress.Id == response.AddressId)
                             || (x.Address.ParentAddressId == response.AddressId
                             || x.Address.Id == response.AddressId))
                             );
            }
            else if (response?.Address?.AdminLevel == 4)
            {
                response2 = response2.Where(x => x.Address.ParentAddressId == response.AddressId
                             || x.Address.Id == response.AddressId
                             );
            }
            else if (response?.Address?.AdminLevel == 5)
            {
                response2 = response2.Where(x => x.Address.Id == response.AddressId);
            }
            if (response2 == null)
            {
                throw new NotFoundException("the requested user does not have team member");

            }

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                response2 = response2.Where(u => EF.Functions.Like(u.UserName, "%" + request.SearchString + "%")
                                            || EF.Functions.Like(u.Email, "%" + request.SearchString + "%")
                                            || EF.Functions.Like(u.PersonalInfo.FirstNameStr, "%" + request.SearchString + "%")
                                            || EF.Functions.Like(u.PersonalInfo.MiddleNameStr, "%" + request.SearchString + "%")
                                            || EF.Functions.Like(u.PersonalInfo.LastNameStr, "%" + request.SearchString + "%")
                                            || EF.Functions.Like(u.PhoneNumber, "%" + request.SearchString + "%"));
            }

            return await PaginatedList<UserResponseDTO>
             .CreateAsync(
                 response2.Where(x => x.Id != response.Id)
                .Select(user => new UserResponseDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Status = user.Status && (!user.LockoutEnabled || user.LockoutEnd == null || user.LockoutEnd <= DateTime.Now),
                    AddressId = user.AddressId,
                    AddressString = _addressService.GetFullAddress(user.Address),
                    GroupName = string.Join(",", user.UserGroups.Select(g => g.GroupName)),
                    FullName = (string)user.PersonalInfo.FullName(true),
                    CreatedDate = _convertor.GregorianToEthiopic(user.CreatedAt),
                    AdminLevel = ((AdminLevel)user.Address.AdminLevel).ToString(),
                    AddressCode = user.Address.Code,
                    CanRegisterEvent = user.CanRegisterEvent,
                    WorkStartedOn=user.Address.WorkStartedOn,
                    PersonalInfo = new PersonalInfoDTO
                    {
                        Id = user.PersonalInfo.Id,
                        FirstName = user.PersonalInfo.FirstNameLang,
                        MiddleName = user.PersonalInfo.MiddleNameLang,

                        // LastName = user.PersonalInfo.LastNameLang,
                        BirthDate = user.PersonalInfo.BirthDate,
                        NationalId = user.PersonalInfo.NationalId,
                        // PlaceOfBirthLookup = user.PersonalInfo.PlaceOfBirthLookup.ValueLang,
                        NationalityLookup = user.PersonalInfo.NationalityLookup.ValueLang,
                        // TitleLookup = user.PersonalInfo.TitleLookup.ValueLang,
                        // ReligionLookup = user.PersonalInfo.ReligionLookup.ValueLang,
                        EducationalStatusLookup = user.PersonalInfo.EducationalStatusLookup.ValueLang,
                        // TypeOfWorkLookup = user.PersonalInfo.TypeOfWorkLookup.ValueLang,
                        MarraigeStatusLookup = user.PersonalInfo.MarraigeStatusLookup.ValueLang,
                        NationLookup = user.PersonalInfo.NationLookup.ValueLang,
                        CreatedDate = user.PersonalInfo.CreatedAt,
                        // ContactInfo = _mapper.Map<ContactInfoDTO>(user.PersonalInfo.ContactInfo)

                    }
                })

                , request.PageCount ?? 1, request.PageSize ?? 10);





            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }
    }
}



