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

namespace AppDiv.CRVS.Application.Features.User.Command.Update
{
    public class CheckEmailDuplicateCommand : IRequest<object>
    {
        public string Email { get; set; }
    }

    public class CheckEmailDuplicateCommandHandler : IRequestHandler<CheckEmailDuplicateCommand, object>
    {
        private readonly IIdentityService _identityService;

        public CheckEmailDuplicateCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<object> Handle(CheckEmailDuplicateCommand request, CancellationToken cancellationToken)
        {
            var res = await _identityService.Exists(request.Email , true);
            return new {exists = res};
        }
    }
}