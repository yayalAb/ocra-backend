using System.Net;
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
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Update
{
    public class UpdateBirthEventCommandValidator : AbstractValidator<UpdateBirthEventCommand>
    {
        public UpdateBirthEventCommandValidator(IEventRepository eventRepo, BirthEvent birth)
        {
            RuleFor(p => p.Id).Must(id => id == birth.Id).WithMessage("Invalid birth Id");
            RuleFor(p => CustomMapper.Mapper.Map<AddBirthEventRequest>(p)).SetValidator(new BirthEventValidator(eventRepo));
            RuleFor(p => p.BirthNotification).SetValidator(new BirthNotificationValidator(eventRepo))
                    .When(p => p.BirthNotification != null);

            RuleFor(p => p.Event.EventOwener.Id).Must(id => id == birth.Event.EventOwener.Id).WithMessage("Invalid birth owener Id");
            RuleFor(p => p.Event.EventOwener).SetValidator(new ChildValidator(eventRepo));

            RuleFor(p => p.Father.Id).Must(id => id == birth.Father.Id).WithMessage("Invalid father Id");
            RuleFor(p => p.Father).SetValidator(new FatherValidator(eventRepo));

            RuleFor(p => p.Mother.Id).Must(id => id == birth.Mother.Id).WithMessage("Invalid mother Id");
            RuleFor(p => p.Mother).SetValidator(new MotherValidator(eventRepo));

            RuleFor(p => p.Event.EventRegistrar).SetValidator(new BirthRegistrarValidator(eventRepo))
                    .When(p => (p.Event.EventRegistrar != null
                            || p.Event.InformantType.ToLower() == "legal guardian"
                            || p.Event.InformantType.ToLower() == "police officer"));
            RuleFor(p => p.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator())
                    .When(p => (p.Event.EventSupportingDocuments != null));
            RuleFor(p => p.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(eventRepo))
                    .When(p => (p.Event.IsExampted));
        }

    }
}