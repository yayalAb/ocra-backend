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

namespace AppDiv.CRVS.Application.Features.User.Query.GetUsersByAddressId
{
    public class GetUsersByAddressIdQuery : IRequest<object>
    {
        public Guid AddressId { get; set; }
        public string? Except {get; set; }
        public bool AddOnlineFlag {get; set;}= false;


    }

    public class GetUsersByAddressIdQueryHandler : IRequestHandler<GetUsersByAddressIdQuery, object>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOnlineUserRepository _onlineUserRepository;



        public GetUsersByAddressIdQueryHandler(IUserRepository userRepository , IOnlineUserRepository onlineUserRepository)
        {
            _userRepository = userRepository;
            _onlineUserRepository = onlineUserRepository;   
        }
        public async Task<object> Handle(GetUsersByAddressIdQuery request, CancellationToken cancellationToken)
        {

            return await _userRepository.GetAll().Where(u => u.AddressId == request.AddressId &&(request.Except == null || u.Id != request.Except))
            .Include(u => u.Address)
            .Select(u => new
            {
                UserId = u.Id,
                AddressId = u.AddressId,
                UserName = u.UserName,
                FullName = u.PersonalInfo.FirstNameLang + " " + u.PersonalInfo.MiddleNameLang + " " + u.PersonalInfo.LastNameLang,
                AddressName = u.Address.AddressNameLang,
                Email = u.Email,
                Online = !request.AddOnlineFlag ?false : _onlineUserRepository.GetAll().Where(ou => ou.UserId == u.Id).Any() 
            }).ToListAsync();


        }
    }
}