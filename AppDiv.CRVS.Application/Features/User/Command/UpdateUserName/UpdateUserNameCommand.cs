using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using MediatR;


namespace AppDiv.CRVS.Application.Features.User.Command.UpdateUserName
{
    public class UpdateUserNameCommand : IRequest<BaseResponse>
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }

    }

    public class UpdateUserNameCommandHandler : IRequestHandler<UpdateUserNameCommand, BaseResponse>
    {
        private readonly IIdentityService _identityService;

        public UpdateUserNameCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<BaseResponse> Handle(UpdateUserNameCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.UserName))
            {
                throw new BadRequestException("both username and email cannot be null or empty");

            }
            var user = await _identityService.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                var res = await _identityService.Exists(request.Email, "email");
                if (res.exists && res.userId != request.UserId)
                {
                    throw new BadRequestException("duplicate email cannot update user");
                }
                user.Email = request.Email;
            }
            if (!string.IsNullOrEmpty(request.UserName))
            {
                var res = await _identityService.Exists(request.UserName, "username");
                if (res.exists && res.userId != request.UserId)
                {
                    throw new BadRequestException("duplicate userName cannot update user");
                }
                user.UserName = request.UserName;
            }
            await _identityService.UpdateAsync(user);
            response.Updated("user");
            return response;
        }
    }
}