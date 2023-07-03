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
    public class ActivateUserCommand : IRequest<BaseResponse>
    {
        public string Id { get; set; }
    }

    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, BaseResponse>
    {
        private readonly IIdentityService _identityService;

        public ActivateUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<BaseResponse> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                var user = await _identityService.GetUserByIdAsync(request.Id);
                user.Status = !user.Status;
                if (user != null)
                {
                    await _identityService.UpdateUserAsync(user);
                    response.Updated("User");
                }
                else
                {
                    throw new NullReferenceException();
                }
                
            }
            catch (NullReferenceException exp)
            {
                response.BadRequest("User Not Found.");
                response.ValidationErrors = new List<string> { exp.Message };
                // throw new System.ApplicationException(exp.Message);
            }
            catch (Exception exp)
            {
                response.BadRequest("Unable to Update.");
                response.ValidationErrors = new List<string> { exp.Message };
                // throw new System.ApplicationException(exp.Message);
            }
            return response;
        }
    }
}