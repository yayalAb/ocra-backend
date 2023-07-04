using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllUser;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.User.Query.GetUserById
{
    public class GetUserByIdQuery : IRequest<FetchSingleUserResponseDTO>
    {
        public string Id { get; private set; }

        public GetUserByIdQuery(string Id)
        {
            this.Id = Id;
        }

    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, FetchSingleUserResponseDTO>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepository;
        private readonly IUserResolverService _userResolverService;

        public GetUserByIdQueryHandler(IIdentityService identityService, IUserRepository userRepository, IUserResolverService userResolverService)
        {
            _identityService = identityService;
            _userRepository = userRepository;
            _userResolverService = userResolverService;
        }
        public async Task<FetchSingleUserResponseDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userResolverService.GetUserId();
            var userData = await _userRepository.GetAll().Where(u => u.Id == request.Id)
            .Include(u => u.UserGroups)
            .Include(u => u.PersonalInfo)
            .ThenInclude(p => p.ContactInfo)
            .Select(u => new FetchSingleUserResponseDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                AddressId = u.AddressId,
                Email = u.Email,
                Otp = u.Otp,
                OtpExpiredDate = u.OtpExpiredDate,
                Status = u.Status &&(!u.LockoutEnabled || u.LockoutEnd==null || u.LockoutEnd <= DateTime.Now),
                UserGroups = u.UserGroups.Select(u => u.Id).ToList(),
                PersonalInfo = CustomMapper.Mapper.Map<UpdatePersonalInfoRequest>(u.PersonalInfo),
                PreferedLanguage = u.PreferedLanguage,
                CreatedBy = u.CreatedBy
            }).FirstOrDefaultAsync();

            if (userData == null)
            {
                throw new NotFoundException($"user with id = {request.Id} is not found");
            }

            return userData;

        }
    }
}