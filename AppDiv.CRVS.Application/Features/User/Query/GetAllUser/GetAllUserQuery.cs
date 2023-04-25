
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Query.GetAllUser

{
    // Customer query with List<Customer> response
    public record GetAllUserQuery : IRequest<List<UserResponseDTO>>
    {

    }

    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, List<UserResponseDTO>>
    {
        private readonly IIdentityService _identityService;

        public GetAllUserQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<List<UserResponseDTO>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            // var LookupList = await _lookupRepository.GetAllAsync();
            // var lookups = CustomMapper.Mapper.Map<List<LookupDTO>>(LookupList);
            // return lookups;

            var userList = await _identityService.AllUsersDetailAsync();
            // foreach (var user in userList)
            // {
            //     user.PersonalInfo.
            // }
            // Console.WriteLine(userList.ToString());
            // Console.ReadLine();
            var users = CustomMapper.Mapper.Map<List<UserResponseDTO>>(userList);

            return users;


            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }
    }
}