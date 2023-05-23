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
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person, IPaymentExamptionRequestRepository ExamptionRequest) _repo;
        // private readonly IMediator _mediator;
        public CreateDeathEventCommandValidator((ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person, IPaymentExamptionRequestRepository ExamptionRequest) repo, CreateDeathEventCommand request)
        {
            _repo = repo;
            // Validate the death event properties.
            RuleFor(p => p.DeathEvent).SetValidator(new DeathEventValidator((_repo.Lookup, _repo.Person)));

            // RuleFor(p => p.DeathEvent.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityLookupId");
            // RuleFor(p => p.DeathEvent.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityTypeLookupId");

            if (request.DeathEvent.DuringDeath != "")
            {
                RuleFor(p => p.DeathEvent.DuringDeath).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "DuringDeath"); ;
            }
            if (request.DeathEvent.DeathNotification != null)
            {
                RuleFor(p => p.DeathEvent.DeathNotification).SetValidator(new DeathNotificationValidator(_repo.Lookup));
            }
            if (!string.IsNullOrEmpty(request.DeathEvent.Event.EventOwener.Id.ToString()) && request.DeathEvent.Event.EventOwener.Id != Guid.Empty)
            {
                RuleFor(p => p.DeathEvent.Event.EventOwener.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person, "EventOwener.Id");
            }
            RuleFor(p => p.DeathEvent.Event.EventOwener).SetValidator(new DeadValidator((_repo.Lookup, _repo.Address)));

            if (request.DeathEvent.Event.EventRegistrar != null)
            {
                if (!string.IsNullOrEmpty(request.DeathEvent.Event.EventRegistrar.RegistrarInfo.Id.ToString()) && request.DeathEvent.Event.EventRegistrar.RegistrarInfo.Id != Guid.Empty)
                {
                    RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.Id.ToString()).ForeignKeyWithPerson(_repo.Person, "EventRegistrar.RegistrarInfo.Id");
                }
                RuleFor(p => p.DeathEvent.Event.EventRegistrar).SetValidator(new DeathRegistrarValidator((_repo.Lookup, _repo.Address)));
            }
            else if (request.DeathEvent.Event.EventRegistrar == null)
            {
                RuleFor(p => p.DeathEvent.Event.EventRegistrar).Must(r => !(r == null)).WithMessage("Registrar Is Required");
            }

            if (request.DeathEvent.Event.EventSupportingDocuments != null)
            {
                // RuleFor(p => p.DeathEvent.Event.EventSupportingDocuments).SupportingDocNull("Event.EventSupportingDocuments").NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator());

            }

            if (request.DeathEvent.Event.PaymentExamption != null)
            {
                RuleFor(p => p.DeathEvent.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(_repo.ExamptionRequest));
                // RuleFor(p => p.DeathEvent.Event.PaymentExamption.ExamptionRequestId.ToString()).NotGuidEmpty().ForeignKeyWithPaymentExamptionRequest(_repo.ExamptionRequest, "Event.PaymentExamption.ExamptionRequestId");
                // RuleFor(p => p.DeathEvent.Event.PaymentExamption.SupportingDocuments).SupportingDocNull("Event.PaymentExamption.EventSupportingDocuments").NotEmpty().NotNull();
            }

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