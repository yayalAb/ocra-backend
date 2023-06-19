using System.Text.RegularExpressions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Domain.Repositories;

namespace AppDiv.CRVS.Application.Features.LoginHistorys.Query
{
    public class GetUserLoginHistory : IRequest<UserHistoryDTO>
    {
        public string userId { set; get; }
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }
    public class GetUserLoginHistoryHandler : IRequestHandler<GetUserLoginHistory, UserHistoryDTO>
    {
        private readonly IUserRepository _LoginHistoryRepo;


        public GetUserLoginHistoryHandler(IUserRepository LoginHistoryRepo)
        {
            _LoginHistoryRepo = LoginHistoryRepo;
        }
        public async Task<UserHistoryDTO> Handle(GetUserLoginHistory request, CancellationToken cancellationToken)
        {
            var UserLogHistory = _LoginHistoryRepo.GetAll()
            .Include(x => x.LoginHistorys)
            .Include(x => x.PersonalInfo)
            .Include(x => x.UserGroups)
            .Include(x => x.Address)
            .Where(x => x.Id == request.userId)
            .Select(x => new UserHistoryDTO
            {
                Id = x.Id,
                Name = x.PersonalInfo.FirstNameLang + " " + x.PersonalInfo.MiddleNameLang + " " + x.PersonalInfo.LastNameLang,
                AssignedOffice = x.Address.AddressNameLang,
                Role = x.UserGroups.Select(g => g.GroupName).FirstOrDefault(),
                Historys = x.LoginHistorys.Select(h => new UserHistoryListDTO
                {
                    EventType = h.EventType,
                    Device = h.Device,
                    IpAddress = h.IpAddress,
                    Date = h.EventDate

                }).ToList()

            }).FirstOrDefault();

            return UserLogHistory;
        }

    }
}




