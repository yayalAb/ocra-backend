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
    public class GetYourTeamQuery : IRequest<List<ApplicationUser>>
    {
        public string UserName { get; set; }
    }

    public class GetYourTeamQueryHandler : IRequestHandler<GetYourTeamQuery, List<ApplicationUser>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContext;

        public GetYourTeamQueryHandler(IHttpContextAccessor httpContext, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _httpContext = httpContext;
        }

        public async Task<List<ApplicationUser>> Handle(GetYourTeamQuery request, CancellationToken cancellationToken)
        {

            var response = _userRepository
            .GetAll()
            .Include(x => x.Address)
            .Where(x => x.UserName == request.UserName).FirstOrDefault();
            if (response.Address.AdminLevel == 2)
            {
                var response2 = _userRepository
                            .GetAll()
                            .Include(x => x.Address)
                            .ThenInclude(x => x.ParentAddress)
                            .ThenInclude(x => x.ParentAddress)
                            .ThenInclude(x => x.ParentAddress).Where(x => x.Address.ParentAddress != null)
                            .Where(x => x.Address.ParentAddress.ParentAddress.ParentAddress.Id == response.AddressId);
                return response2.ToList();
            }
            if (response.Address.AdminLevel == 2)
            {
                var response2 = _userRepository

                            .GetAll()
                            .Include(x => x.Address)
                            .Where(x => x.Address.ParentAddress.ParentAddress.Id == response.AddressId);
                return response2.ToList();
            }
            if (response.Address.AdminLevel == 3)
            {
                var response2 = _userRepository

                            .GetAll()
                            .Include(x => x.Address)
                            .Where(x => x.Address.ParentAddress.Id == response.AddressId);
                return response2.ToList();
            }
            if (response.Address.AdminLevel == 4)
            {
                var response2 = _userRepository

                            .GetAll()
                            .Include(x => x.Address)
                            .Where(x => x.AddressId == response.AddressId);
                return response2.ToList();
            }


            var res = new List<ApplicationUser>();
            return res;
        }
    }
}