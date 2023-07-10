using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Validators;
using FluentValidation;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Update
{
    // Validator for birth update command.
    public class UpdateBirthEventCommandValidator : AbstractValidator<UpdateBirthEventCommand>
    {
        public UpdateBirthEventCommandValidator(IEventRepository eventRepo)
        {
            // Validate inputs.
            RuleFor(p => CustomMapper.Mapper.Map<AddBirthEventRequest>(p)).SetValidator(new BirthEventValidator(eventRepo));
            RuleFor(p => p.BirthNotification).SetValidator(new BirthNotificationValidator(eventRepo)!)
                    .When(p => p.BirthNotification != null);
            RuleFor(p => p.Event.EventOwener).SetValidator(new ChildValidator(eventRepo));
            RuleFor(p => p.Father).SetValidator(new FatherValidator(eventRepo));
            RuleFor(p => p.Mother).SetValidator(new MotherValidator(eventRepo));
            RuleFor(p => p.Event.EventRegistrar).SetValidator(new BirthRegistrarValidator(eventRepo)!)
                    .When(p => (p.Event.EventRegistrar != null
                            || p.Event.InformantType?.ToLower() == "legal guardian"
                            || p.Event.InformantType?.ToLower() == "police officer"));
            RuleFor(p => p.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator()!)
                    .When(p => (p.Event.EventSupportingDocuments != null));
            RuleFor(p => p.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(eventRepo)!)
                    .When(p => (p.Event.IsExampted));
        }

    }
}