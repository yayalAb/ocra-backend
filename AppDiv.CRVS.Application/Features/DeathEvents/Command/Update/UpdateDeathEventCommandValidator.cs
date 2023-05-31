using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
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

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Update
{
    public class UpdateDeathEventCommandValidator : AbstractValidator<UpdateDeathEventCommand>
    {
        public UpdateDeathEventCommandValidator(IEventRepository eventRepo)
        {
            RuleFor(p => CustomMapper.Mapper.Map<AddDeathEventRequest>(p)).SetValidator(new DeathEventValidator(eventRepo));

            RuleFor(p => p.DeathNotification).SetValidator(new DeathNotificationValidator(eventRepo))
                    .When(p => p.DeathNotification != null);
            RuleFor(p => p.Event.EventOwener).SetValidator(new DeadValidator(eventRepo));

            RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.Id.ToString()).ForeignKeyWithPerson(eventRepo, "EventRegistrar.RegistrarInfo.Id")
                    .When(p => p.Event.EventRegistrar != null);
            RuleFor(p => p.Event.EventRegistrar).SetValidator(new DeathRegistrarValidator(eventRepo))
                    .When(p => p.Event.EventRegistrar != null);
            RuleFor(p => p.Event.EventRegistrar).Must(r => !(r == null)).WithMessage("Registrar Is Required")
                    .When(p => p.Event.EventRegistrar == null);

            RuleFor(p => p.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator())
                    .When(p => (p.Event.EventSupportingDocuments != null));

            RuleFor(p => p.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(eventRepo))
                    .When(p => (p.Event.IsExampted));
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