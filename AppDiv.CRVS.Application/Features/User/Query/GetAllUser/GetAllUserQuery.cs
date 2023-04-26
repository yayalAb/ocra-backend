
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AutoMapper;
using MediatR;
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
        // private readonly IMapper _mapper;

        public GetAllUserQueryHandler(IIdentityService identityService )
        {
            _identityService = identityService;
            // _mapper = mapper;
        }
        public async Task<PaginatedList<UserResponseDTO>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<UserResponseDTO>
             .CreateAsync(
                 _identityService
                .AllUsersDetail()
                .Select(user => new UserResponseDTO{
                    Id = user.Id ,
                    UserName = user.UserName,
                    Email = user.Email,
                    PersonalInfo = new PersonalInfoDTO{
                        Id = user.PersonalInfo.Id,
                        FirstName = user.PersonalInfo.FirstName.Value<string>("en"),
                        MiddleName = user.PersonalInfo.MiddleName.Value<string>("en"),
                        // LastName = user.PersonalInfo.LastName.Value<string>("en"),
                        BirthDate = user.PersonalInfo.BirthDate,
                        NationalId = user.PersonalInfo.NationalId,
                        // PlaceOfBirthLookup = user.PersonalInfo.PlaceOfBirthLookup.Value.Value<string>("en"),
                        NationalityLookup = user.PersonalInfo.NationalityLookup.Value.Value<string>("en"),
                        // TitleLookup = user.PersonalInfo.TitleLookup.Value.Value<string>("en"),
                        // ReligionLookup = user.PersonalInfo.ReligionLookup.Value.Value<string>("en"),
                        EducationalStatusLookup = user.PersonalInfo.EducationalStatusLookup.Value.Value<string>("en"),
                        // TypeOfWorkLookup = user.PersonalInfo.TypeOfWorkLookup.Value.Value<string>("en"),
                        MarraigeStatusLookup = user.PersonalInfo.MarraigeStatusLookup.Value.Value<string>("en"),
                        NationLookup = user.PersonalInfo.NationLookup.Value.Value<string>("en"),
                        CreatedDate = user.PersonalInfo.CreatedAt,
                        // ContactInfo = _mapper.Map<ContactInfoDTO>(user.PersonalInfo.ContactInfo)

                    }
                }).ToList()
                
                , request.PageCount ?? 1, request.PageSize ?? 10);





            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }
    }
}