using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Validators
{
    public class BirthNotificationValidator : AbstractValidator<AddBirthNotificationRequest>
    {
        private readonly ILookupRepository _repo;
        public BirthNotificationValidator(ILookupRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.DeliveryTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "DeliveryTypeLookupId");
            RuleFor(p => p.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "SkilledProfLookupId");
            RuleFor(p => p.WeightAtBirth).NotEmpty().NotNull();
        }
    }
    public class DeathNotificationValidator : AbstractValidator<AddDeathNotificationRequest>
    {
        private readonly ILookupRepository _repo;
        public DeathNotificationValidator(ILookupRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.CauseOfDeathInfoTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "CauseOfDeathInfoTypeLookupId");
            // RuleFor(p => p.DeathNotification.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.CauseOfDeath).NotEmpty().NotNull();
            RuleFor(p => p.CauseOfDeathInfoTypeLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "CauseOfDeathInfoTypeLookupId");
            RuleFor(p => p.DeathNotificationSerialNumber).NotEmpty().NotNull();
        }
    }






}