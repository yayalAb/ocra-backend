using System.Net.Cache;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Validators;
using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    public class CreateBirthEventCommandValidator : AbstractValidator<CreateBirthEventCommand>
    {
        public CreateBirthEventCommandValidator(IEventRepository eventRepo)
        {

            RuleFor(p => p.BirthEvent).SetValidator(new BirthEventValidator(eventRepo));
            RuleFor(p => p.BirthEvent.Event.EventOwener).SetValidator(new ChildValidator(eventRepo));
            RuleFor(p => p.BirthEvent.Father).SetValidator(new FatherValidator(eventRepo));
            RuleFor(p => p.BirthEvent.Mother).SetValidator(new MotherValidator(eventRepo));
            RuleFor(p => p.BirthEvent.BirthNotification).SetValidator(new BirthNotificationValidator(eventRepo))
                    .When(p => p.BirthEvent.BirthNotification != null);
            RuleFor(p => p.BirthEvent.Event.EventRegistrar).SetValidator(new BirthRegistrarValidator(eventRepo))
                    .When(p => (p.BirthEvent.Event.EventRegistrar != null
                                || p.BirthEvent.Event.InformantType.ToLower() == "legal guardian"
                                || p.BirthEvent.Event.InformantType.ToLower() == "police officer"));
            RuleFor(p => p.BirthEvent.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator())
                    .When(p => (p.BirthEvent.Event.EventSupportingDocuments != null));
            RuleFor(p => p.BirthEvent.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(eventRepo))
                .When(p => (p.BirthEvent.Event.IsExampted));
        }

    }
}