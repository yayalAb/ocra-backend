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
            RuleFor(p => p.CauseOfDeathInfoTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "CauseOfDeathInfoTypeLookupId");
            // RuleFor(p => p.DeathNotification.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.CauseOfDeathArray).NotEmpty().NotNull();
            RuleFor(p => p.CauseOfDeathInfoTypeLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "CauseOfDeathInfoTypeLookupId");
            RuleFor(p => p.DeathNotificationSerialNumber).NotEmpty().NotNull();
        }
    }






}