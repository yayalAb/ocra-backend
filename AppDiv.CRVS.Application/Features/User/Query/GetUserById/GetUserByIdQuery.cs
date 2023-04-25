using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllUser;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Mapper;
using MediatR;

namespace AppDiv.CRVS.Application.Features.User.Query.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserResponseDTO>
    {
        public string Id { get; private set; }

        public GetUserByIdQuery(string Id)
        {
            this.Id = Id;
        }

    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponseDTO>
    {
        private readonly IIdentityService _identityService;

        public GetUserByIdQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<UserResponseDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            // var users = await _identityService.Send(new GetAllUserQuery());
            var user = await _identityService.GetUserByIdAsync(request.Id.ToString());
            return CustomMapper.Mapper.Map<UserResponseDTO>(user);
            // return user;
        }
    }
}