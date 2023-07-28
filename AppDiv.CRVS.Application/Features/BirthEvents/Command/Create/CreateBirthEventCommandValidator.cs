using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Validators;
using FluentValidation;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    // Validator for birth event create.
    public class CreateBirthEventCommandValidator : AbstractValidator<CreateBirthEventCommand>
    {
        public CreateBirthEventCommandValidator(IEventRepository eventRepo)
        {
            // Date converter from Ethiopian date to Gregorian date and vice versa.
            var dateConverter = new CustomDateConverter();
            // Validate the inputs.
            RuleFor(p => p.BirthEvent).SetValidator(new BirthEventValidator(eventRepo));
            RuleFor(p => p.BirthEvent.Event.EventOwener).SetValidator(new ChildValidator(eventRepo));
            RuleFor(p => p.BirthEvent.Father).SetValidator(new FatherValidator(eventRepo)!)
            .When(p => p.BirthEvent.Father != null);
            RuleFor(p => p.BirthEvent.Mother).SetValidator(new MotherValidator(eventRepo)!)
            .When(p => p.BirthEvent.Mother != null);
            RuleFor(p => p.BirthEvent.Event.CertificateId).NotEmpty().NotNull().ValidCertificate(eventRepo, "Event.CertificateId", "Birth")
            .When(p => p.BirthEvent.Event.CertificateId != null);
            RuleFor(p => p.BirthEvent.BirthNotification).SetValidator(new BirthNotificationValidator(eventRepo)!)
                    .When(p => p.BirthEvent.BirthNotification != null);
            RuleFor(p => p.BirthEvent.Event.EventRegistrar).SetValidator(new BirthRegistrarValidator(eventRepo)!)
                    .When(p => (p.BirthEvent.Event.EventRegistrar != null
                                || p.BirthEvent.Event.InformantType?.ToLower() == "legal guardian"
                                || p.BirthEvent.Event.InformantType?.ToLower() == "police officer"));
            RuleFor(p => p.BirthEvent.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator()!)
                    .When(p => (p.BirthEvent.Event.EventSupportingDocuments != null));
            RuleFor(p => p.BirthEvent.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(eventRepo)!)
                .When(p => (p.BirthEvent.Event.IsExampted));
        }

    }
}