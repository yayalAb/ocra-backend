
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.ProfileChangeRequests.Commands.Create
{
    public class CreateProfileChangeRequestCommandValidetor : AbstractValidator<CreateProfileChangeRequestCommand>
    {
        private readonly IProfileChangeRequestRepository _repo;
        public CreateProfileChangeRequestCommandValidetor(IProfileChangeRequestRepository repo)
        {
            _repo = repo;
        }
    }
}