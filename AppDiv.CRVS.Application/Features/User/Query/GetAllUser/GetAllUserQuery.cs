
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
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
    }

    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, PaginatedList<UserResponseDTO>>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserResolverService _UserResolver;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContext;

        // private readonly IMapper _mapper;

        public GetAllUserQueryHandler(IHttpContextAccessor httpContext, IUserRepository userRepository, IIdentityService identityService, IUserResolverService UserResolver)
        {
            _identityService = identityService;
            _UserResolver = UserResolver;
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
            .GetAll()
            .Include(x => x.Address)
            .Include(x => x.PersonalInfo)
            .Where(x => x.PersonalInfoId == userId).FirstOrDefault();
            var response2 = _userRepository
                               .GetAll()
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





            return await PaginatedList<UserResponseDTO>
             .CreateAsync(
                 response2.Where(x => x.PersonalInfoId == userId)
                .Select(user => new UserResponseDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Status = user.Status && (!user.LockoutEnabled || user.LockoutEnd == null || user.LockoutEnd <= DateTime.Now),
                    AddressId = user.AddressId,
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



