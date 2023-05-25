using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllUser;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;

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

        public GetUserByIdQueryHandler(IIdentityService identityService , IUserRepository userRepository)
        {
            _identityService = identityService;
            _userRepository = userRepository;
        }
        public async Task<FetchSingleUserResponseDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
              var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
                                                {
                                                    { "UserGroups", NavigationPropertyType.COLLECTION },
                                                    { "PersonalInfo", NavigationPropertyType.REFERENCE }

                                                  
                                                };
            var userData = await _userRepository.GetWithAsync(request.Id,explicitLoadedProperties);



            return new FetchSingleUserResponseDTO{
                Id = userData.Id,
                UserName = userData.UserName,
                AddressId = userData.AddressId,
                Email = userData.Email,
                UserGroups = userData.UserGroups.Select(u => u.Id).ToList(),
                PersonalInfo = CustomMapper.Mapper.Map<AddPersonalInfoRequest>(userData.PersonalInfo)

            };

        }
    }
}