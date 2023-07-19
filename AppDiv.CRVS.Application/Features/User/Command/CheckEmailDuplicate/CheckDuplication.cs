using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.User.Command.CheckDuplication
{
    public class CheckDuplicationCommand : IRequest<object>
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? GroupName { get; set; }
    }

    public class CheckDuplicationCommandHandler : IRequestHandler<CheckDuplicationCommand, object>
    {
        private readonly IIdentityService _identityService;
        private readonly IGroupRepository _group;

        public CheckDuplicationCommandHandler(IIdentityService identityService, IGroupRepository group)
        {
            _identityService = identityService;
            this._group = group;
        }
        public async Task<object> Handle(CheckDuplicationCommand request, CancellationToken cancellationToken)
        {
            if (request.Email == null && request.UserName == null && request.PhoneNumber == null && request.GroupName == null) 
            {
                throw new BadRequestException("Atleast one check criteria(email , username , phoneNumber, GroupName) is needed to check for duplication ");
            }
            var res = request.Email != null
                        ? await _identityService.Exists(request.Email, "email")
                        :request.UserName != null
                        ? await _identityService.Exists(request.UserName, "username")
                        :request.PhoneNumber != null
                        ? await _identityService.Exists(request.PhoneNumber, "phone")
                        :request.GroupName != null
                        ? await _group.AnyAsync(g => g.GroupName.Replace(" ", "") == request.GroupName.Replace(" ", ""))
                        :false;
            return new { exists = res };
        }
    }
}