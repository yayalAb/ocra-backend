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
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person, IPaymentExamptionRequestRepository ExamptionRequest) _repo;

        public CreateBirthEventCommandValidator((ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person, IPaymentExamptionRequestRepository ExamptionRequest) repo, CreateBirthEventCommand request
        // , ILogger<CreateBirthEventCommandValidator> log
        )
        {
            _repo = repo;

            RuleFor(p => p.BirthEvent).SetValidator(new BirthEventValidator((_repo.Lookup, _repo.Person)));
            RuleFor(p => p.BirthEvent.Event.EventOwener).SetValidator(new ChildValidator((_repo.Lookup, _repo.Address), request.BirthEvent.Father));
            RuleFor(p => p.BirthEvent.Father).SetValidator(new FatherValidator((_repo.Lookup, _repo.Address)));
            RuleFor(p => p.BirthEvent.Mother).SetValidator(new MotherValidator((_repo.Lookup, _repo.Address)));

            if (request.BirthEvent.BirthNotification != null)
            {
                RuleFor(p => p.BirthEvent.BirthNotification).SetValidator(new BirthNotificationValidator(_repo.Lookup));
            }
            if (request.BirthEvent.Event.EventRegistrar != null)
            {
                RuleFor(p => p.BirthEvent.Event.EventRegistrar).SetValidator(new BirthRegistrarValidator((_repo.Lookup, _repo.Address)));
            }

            // }
            if (request.BirthEvent.Event.EventSupportingDocuments != null)
            {
                RuleFor(p => p.BirthEvent.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator());
            }

            if (request.BirthEvent.Event.PaymentExamption != null)
            {
                RuleFor(p => p.BirthEvent.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(_repo.ExamptionRequest));
            }
        }

    }
}