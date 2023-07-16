using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Validators
{
    public class DeathNotificationValidator : AbstractValidator<AddDeathNotificationRequest>
    {
        private readonly IEventRepository _repo;
        public DeathNotificationValidator(IEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.CauseOfDeathInfoTypeLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "CauseOfDeathInfoTypeLookupId")
            .When(p => p.CauseOfDeathInfoTypeLookupId != null);
            // RuleFor(p => p.DeathNotification.SkilledProfLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.CauseOfDeathArray).NotEmpty().NotNull()
            .When(p => p.CauseOfDeathArray != null);
            RuleFor(p => p.CauseOfDeathInfoTypeLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "CauseOfDeathInfoTypeLookupId")
            .When(p => p.CauseOfDeathInfoTypeLookupId != null);
            RuleFor(p => p.DeathNotificationSerialNumber).NotEmpty().NotNull()
            .When(p => p.DeathNotificationSerialNumber != null);
        }
    }






}