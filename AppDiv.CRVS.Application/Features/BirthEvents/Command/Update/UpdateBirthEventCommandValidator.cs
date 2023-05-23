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
        // private readonly BirthEvent _birth;
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person, IPaymentExamptionRequestRepository ExamptionRequest) _repo;

        public UpdateBirthEventCommandValidator((ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person, IPaymentExamptionRequestRepository ExamptionRequest) repo,
                                                BirthEvent birth, UpdateBirthEventCommand request)
        {
            _repo = repo;
            RuleFor(p => p.Id).Must(id => id == birth.Id).WithMessage("Invalid birth Id");
            RuleFor(p => CustomMapper.Mapper.Map<AddBirthEventRequest>(p)).SetValidator(new BirthEventValidator((_repo.Lookup, _repo.Person)));
            if (request.BirthNotification != null)
            {
                // RuleFor(p => p.Id).Must(id => id == birth.BirthNotification.Id).WithMessage("Invalid birth Notification Id");
                RuleFor(p => p.BirthNotification).SetValidator(new BirthNotificationValidator(_repo.Lookup));
            }
            // RuleFor(p => p.Event.EventOwener.Id).Must(id => id == birth.Event.EventOwener.Id).WithMessage("Invalid birth owener Id");
            RuleFor(p => p.Event.EventOwener).SetValidator(new ChildValidator((_repo.Lookup, _repo.Address), request.Father));

            // RuleFor(p => p.Father.Id).Must(id => id == birth.Father.Id).WithMessage("Invalid father Id");
            RuleFor(p => p.Father).SetValidator(new FatherValidator((_repo.Lookup, _repo.Address)));

            // RuleFor(p => p.Mother.Id).Must(id => id == birth.Mother.Id).WithMessage("Invalid mother Id");
            RuleFor(p => p.Mother).SetValidator(new MotherValidator((_repo.Lookup, _repo.Address)));

            if (request.Event.EventRegistrar != null)
            {
                // RuleFor(p => p.Event.EventRegistrar.Id).Must(id => id == birth.Event.EventRegistrar.Id).WithMessage("Invalid registrar Id");
                // RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.Id).Must(id => id == birth.Event.EventRegistrar.RegistrarInfo.Id).WithMessage("Invalid registrar Person Id");
                RuleFor(p => p.Event.EventRegistrar).SetValidator(new BirthRegistrarValidator((_repo.Lookup, _repo.Address)));
            }
            // }
            if (request.Event.EventSupportingDocuments != null)
            {
                // RuleFor(p => p.Event.EventSupportingDocuments.Id).Must(id => id == birth.Event.EventSupportingDocuments.Id).WithMessage("Invalid registrar Id");
                RuleFor(p => p.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator());
            }

            if (request.Event.PaymentExamption != null)
            {
                // RuleFor(p => p.Event.PaymentExamption.Id).Must(id => id == birth.Event.PaymentExamption.Id).WithMessage("Invalid paymentExamption Id");
                RuleFor(p => p.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(_repo.ExamptionRequest));
            }
        }

    }
}