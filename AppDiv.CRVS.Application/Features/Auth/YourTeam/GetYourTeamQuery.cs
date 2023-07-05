using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using MediatR;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Contracts;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using AppDiv.CRVS.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.Auth.YourTeam

{
    public class GetYourTeamQuery : IRequest<PaginatedList<YourTeamDTO>>
    {
        public string UserName { get; set; }
        public int? PageCount { get; set; }
        public int? PageSize { get; set; }
        public string? SearchString { get; set; }
    }

    public class GetYourTeamQueryHandler : IRequestHandler<GetYourTeamQuery, PaginatedList<YourTeamDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContext;

        public GetYourTeamQueryHandler(IHttpContextAccessor httpContext, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _httpContext = httpContext;
        }

        public async Task<PaginatedList<YourTeamDTO>> Handle(GetYourTeamQuery request, CancellationToken cancellationToken)
        {

            var response = _userRepository
            .GetAll()
            .Include(x => x.Address)
            .Include(x => x.PersonalInfo)
            .Where(x => x.UserName == request.UserName).FirstOrDefault();
            if (response == null)
            {
                throw new NotFoundException("User Name Does not Found ");
            }

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
                throw new NotFoundException("the requested user have not team member");

            }
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                response2 = response2.Where(
                    u => EF.Functions.Like(u.UserName, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.UserGroups.Select(g => g.GroupName).FirstOrDefault()!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Address.AddressNameStr, "%" + request.SearchString + "%")
                         );
            }
            var result = response2.Where(x => x.Id != response.Id).Select(x => new YourTeamDTO
            {
                Id = x.Id,
                UserName = x.UserName,
                UserGroup = x.UserGroups.Select(x => x.GroupName).FirstOrDefault(),
                AddressName = x.Address.AddressNameLang,
                AddressId = x.Address.Id,
                Email = x.Email,
                Status = x.Status && (!x.LockoutEnabled || x.LockoutEnd == null || x.LockoutEnd <= DateTime.Now),
                PersonalInfo = new PersonalInfoDTO
                {
                    Id = x.PersonalInfo.Id,
                    FirstName = x.PersonalInfo.FirstNameLang,
                    MiddleName = x.PersonalInfo.MiddleNameLang,

                    // LastName = x.PersonalInfo.LastNameLang,
                    BirthDate = x.PersonalInfo.BirthDate,
                    NationalId = x.PersonalInfo.NationalId,
                    // PlaceOfBirthLookup = x.PersonalInfo.PlaceOfBirthLookup.ValueLang,
                    NationalityLookup = x.PersonalInfo.NationalityLookup.ValueLang,
                    // TitleLookup = x.PersonalInfo.TitleLookup.ValueLang,
                    // ReligionLookup = x.PersonalInfo.ReligionLookup.ValueLang,
                    EducationalStatusLookup = x.PersonalInfo.EducationalStatusLookup.ValueLang,
                    // TypeOfWorkLookup = x.PersonalInfo.TypeOfWorkLookup.ValueLang,
                    MarraigeStatusLookup = x.PersonalInfo.MarraigeStatusLookup.ValueLang,
                    NationLookup = x.PersonalInfo.NationLookup.ValueLang,
                    CreatedDate = x.PersonalInfo.CreatedAt,
                    // ContactInfo = _mapper.Map<ContactInfoDTO>(x.PersonalInfo.ContactInfo)

                }
            });

            return await PaginatedList<YourTeamDTO>
                            .CreateAsync(
                                 result
                                , request.PageCount ?? 1, request.PageSize ?? 10);


        }
    }
}
