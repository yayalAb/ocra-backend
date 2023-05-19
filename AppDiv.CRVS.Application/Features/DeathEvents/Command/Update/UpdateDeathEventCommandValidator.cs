using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Service;
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
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person, IPaymentExamptionRequestRepository ExamptionRequest) _repo;
        // private readonly IMediator _mediator;
        public UpdateDeathEventCommandValidator((ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person, IPaymentExamptionRequestRepository ExamptionRequest) repo, UpdateDeathEventCommand request)
        {
            _repo = repo;
            RuleFor(p => p.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityLookupId");
            RuleFor(p => p.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityTypeLookupId");
            // RuleFor(p => p.BirthPlaceId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo);
            // RuleFor(p => p.DuringDeath).NotEmpty().NotNull();
            RuleFor(p => p.BirthCertificateId).NotEmpty().NotNull();
            RuleFor(p => p.PlaceOfFuneral).NotEmpty().NotNull();
            RuleFor(p => p.DeathNotification.CauseOfDeathInfoTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "CauseOfDeathInfoTypeLookupId");
            // RuleFor(p => p.DeathNotification.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.DeathNotification.CauseOfDeath).NotEmpty().NotNull();
            RuleFor(p => p.DeathNotification.CauseOfDeathInfoTypeLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo.Lookup, "CauseOfDeathInfoTypeLookupId");
            RuleFor(p => p.DeathNotification.DeathNotificationSerialNumber).NotEmpty().NotNull();
            RuleFor(p => p.Event.RegBookNo).NotEmpty().NotNull();
            RuleFor(p => p.Event.CivilRegOfficeCode).NotEmpty().NotNull();
            RuleFor(p => p.Event.CertificateId).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventRegDate).NotEmpty().NotNull().Must(date => date < DateTime.Now && date > new DateTime(1900, 1, 1));
            RuleFor(p => p.Event.CivilRegOfficerId.ToString()).NotEmpty().NotNull().ForeignKeyWithPerson(_repo.Person, "CivilRegOfficerId");
            RuleFor(p => p.Event.EventOwener.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
            RuleFor(p => p.Event.EventOwener.TitleLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "TitleLookupId");
            RuleFor(p => p.Event.EventOwener.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "NationalityLookupId");
            RuleFor(p => p.Event.EventOwener.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "ResidentAddressId");
            RuleFor(p => p.Event.EventOwener.ReligionLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "ReligionLookupId");
            RuleFor(p => p.Event.EventOwener.EducationalStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "EducationalStatusLookupId");
            RuleFor(p => p.Event.EventOwener.TypeOfWorkLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "TypeOfWorkLookupId");
            RuleFor(p => p.Event.EventOwener.MarriageStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "MarriageStatusLookupId");
            RuleFor(p => p.Event.EventOwener.NationLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "NationLookupId");


            if (request.Event.EventRegistrar != null)
            {
                // RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person);
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.LastName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "EventRegistrar.RegistrarInfo.SexLookupId");
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "EventRegistrar.RegistrarInfo.ResidentAddressId");
                // RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.BirthDate).NotEmpty().NotNull()
                // .Must(date => date < DateTime.Now && date > necw DateTime(1900, 1, 1));
            }
            else if (request.Event.EventRegistrar == null)
            {
                RuleFor(p => p.Event.EventRegistrar).Must(r => !(r == null)).WithMessage("Registrar Is Required");
            }

            if (request.Event.EventSupportingDocuments != null)
            {
                RuleFor(p => p.Event.EventSupportingDocuments).SupportingDocNull("Event.EventSupportingDocuments").NotEmpty().NotNull();
            }

            if (request.Event.PaymentExamption != null)
            {
                RuleFor(p => p.Event.PaymentExamption.ExamptionRequestId.ToString()).NotGuidEmpty().ForeignKeyWithPaymentExamptionRequest(_repo.ExamptionRequest, "Event.PaymentExamption.ExamptionRequestId");
                RuleFor(p => p.Event.PaymentExamption.SupportingDocuments).SupportingDocNull("Event.PaymentExamption.EventSupportingDocuments").NotEmpty().NotNull();
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