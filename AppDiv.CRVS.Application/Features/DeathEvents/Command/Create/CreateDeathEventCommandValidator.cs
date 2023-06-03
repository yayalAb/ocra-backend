using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Validators;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Create
{
    public class CreateDeathEventCommandValidator : AbstractValidator<CreateDeathEventCommand>
    {
        public CreateDeathEventCommandValidator(IEventRepository eventRepo)
        {
            // Validate the death event properties.
            RuleFor(p => p.DeathEvent).SetValidator(new DeathEventValidator(eventRepo));
            RuleFor(p => p.DeathEvent.DuringDeath).NotGuidEmpty().ForeignKeyWithLookup(eventRepo, "DuringDeath")
                    .When(p => p.DeathEvent.DuringDeath != null);
            RuleFor(p => p.DeathEvent.DeathNotification).SetValidator(new DeathNotificationValidator(eventRepo))
                    .When(p => p.DeathEvent.DeathNotification != null);
            RuleFor(p => p.DeathEvent.Event.EventOwener).SetValidator(new DeadValidator(eventRepo));
            RuleFor(p => p.DeathEvent.Event.EventRegistrar).SetValidator(new DeathRegistrarValidator(eventRepo))
                    .When(p => p.DeathEvent.Event.EventRegistrar != null);
            RuleFor(p => p.DeathEvent.Event.EventRegistrar).Must(r => !(r == null)).WithMessage("Registrar Is Required")
                    .When(p => p.DeathEvent.Event.EventRegistrar == null);
            RuleFor(p => p.DeathEvent.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator())
                    .When(p => (p.DeathEvent.Event.EventSupportingDocuments != null));
            RuleFor(p => p.DeathEvent.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(eventRepo))
                    .When(p => (p.DeathEvent.Event.IsExampted));

        }

        //private async Task<bool> phoneNumberUnique(CreateCustomerCommand request, CancellationToken token)
        //{
        //    var member = await _repo.GetByIdAsync(request.FirstName);
        //    if (member == null)
        //        return true;
        //    else return false;
        //}

    }
}