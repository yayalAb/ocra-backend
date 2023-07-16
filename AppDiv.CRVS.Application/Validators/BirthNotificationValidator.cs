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
            RuleFor(p => p.DeliveryTypeLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "DeliveryTypeLookupId")
            .When(p => p.DeliveryTypeLookupId != null);
            RuleFor(p => p.SkilledProfLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "SkilledProfLookupId")
            .When(p => p.SkilledProfLookupId != null);
            RuleFor(p => p.WeightAtBirth).NotEmpty().NotNull()
            .When(p => p.WeightAtBirth != null);
        }
    }
}