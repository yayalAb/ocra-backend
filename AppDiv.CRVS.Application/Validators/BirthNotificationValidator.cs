using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Validators
{
    public class BirthNotificationValidator : AbstractValidator<AddBirthNotificationRequest>
    {
        private readonly IEventRepository _repo;
        public BirthNotificationValidator(IEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.DeliveryTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "DeliveryTypeLookupId");
            RuleFor(p => p.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "SkilledProfLookupId");
            RuleFor(p => p.WeightAtBirth).NotEmpty().NotNull();
        }
    }
}