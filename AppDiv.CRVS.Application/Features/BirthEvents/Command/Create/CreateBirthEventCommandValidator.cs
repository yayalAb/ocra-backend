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
            if (!string.IsNullOrEmpty(request.BirthEvent.Event.EventOwener.Id.ToString()) && request.BirthEvent.Event.EventOwener.Id != Guid.Empty)
            {
                RuleFor(p => p.BirthEvent.Event.EventOwener.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person, "EventOwener.Id");
            }
            RuleFor(p => p.BirthEvent.Event.EventOwener).SetValidator(new ChildValidator((_repo.Lookup, _repo.Address), request.BirthEvent.Father));
            if (!string.IsNullOrEmpty(request.BirthEvent.Father.Id.ToString()) && request.BirthEvent.Father.Id != Guid.Empty)
            {
                RuleFor(p => p.BirthEvent.Father.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person, "Father.Id");
            }
            RuleFor(p => p.BirthEvent.Father).SetValidator(new FatherValidator((_repo.Lookup, _repo.Address)));
            if (!string.IsNullOrEmpty(request.BirthEvent.Mother.Id.ToString()) && request.BirthEvent.Mother.Id != Guid.Empty)
            {
                RuleFor(p => p.BirthEvent.Mother.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person, "Mother.Id");
            }
            RuleFor(p => p.BirthEvent.Mother).SetValidator(new MotherValidator((_repo.Lookup, _repo.Address)));

            if (request.BirthEvent.BirthNotification != null)
            {
                RuleFor(p => p.BirthEvent.BirthNotification).SetValidator(new BirthNotificationValidator(_repo.Lookup));
            }
            if (request.BirthEvent.Event.EventRegistrar != null || request.BirthEvent.Event.InformantType.ToLower() == "legal guardian"
                || request.BirthEvent.Event.InformantType.ToLower() == "police officer")
            {
                if (!string.IsNullOrEmpty(request.BirthEvent.Event.EventRegistrar.RegistrarInfo.Id.ToString()) && request.BirthEvent.Event.EventRegistrar.RegistrarInfo.Id != Guid.Empty)
                {
                    RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.Id.ToString()).ForeignKeyWithPerson(_repo.Person, "EventRegistrar.RegistrarInfo.Id");
                }
                RuleFor(p => p.BirthEvent.Event.EventRegistrar).SetValidator(new BirthRegistrarValidator((_repo.Lookup, _repo.Address)));
            }

            // }
            if (request.BirthEvent.Event.EventSupportingDocuments != null)
            {
                RuleFor(p => p.BirthEvent.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator());
            }

            if (request.BirthEvent.Event.PaymentExamption != null && request.BirthEvent.Event.IsExampted)
            {
                RuleFor(p => p.BirthEvent.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(_repo.ExamptionRequest));
            }
        }

    }
}