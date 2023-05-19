using System.Net;
using System.Net.Cache;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Application.Service;
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

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Update
{
    public class UpdateBirthEventCommandValidator : AbstractValidator<UpdateBirthEventCommand>
    {
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person, IPaymentExamptionRequestRepository ExamptionRequest) _repo;
        // private readonly IMediator _mediator;
        // private readonly ILogger<UpdateBirthEventCommandValidator> log;
        public UpdateBirthEventCommandValidator((ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person, IPaymentExamptionRequestRepository ExamptionRequest) repo, UpdateBirthEventCommand request
        // , ILogger<CreateBirthEventCommandValidator> log
        )
        {
            _repo = repo;

            RuleFor(p => p.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityLookupId");
            RuleFor(p => p.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityTypeLookupId");
            RuleFor(p => p.BirthPlaceId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "BirthPlaceId");
            RuleFor(p => p.TypeOfBirthLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "TypeOfBirthLookupId");
            RuleFor(p => p.BirthNotification.DeliveryTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "DeliveryTypeLookupId");
            RuleFor(p => p.BirthNotification.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SkilledProfLookupId");
            RuleFor(p => p.BirthNotification.WeightAtBirth).NotEmpty().NotNull();
            RuleFor(p => p.Event.RegBookNo).NotEmpty().NotNull();
            RuleFor(p => p.Event.CivilRegOfficeCode).NotEmpty().NotNull();
            RuleFor(p => p.Event.CertificateId).NotEmpty().NotNull();
            RuleFor(p => p.Event.CivilRegOfficerId.ToString()).NotEmpty().NotNull().ForeignKeyWithPerson(_repo.Person, "CivilRegOfficerId");
            RuleFor(p => p.Event.EventOwener.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.MiddleName.or).Must(f => f == request.Father.FirstName.or).WithMessage("The child's father name and his father first name does not match.").NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.MiddleName.am).Must(f => f == request.Father.FirstName.am).WithMessage("The child's father's name and his father's first name do not match.").NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.LastName.or).Must(f => f == request.Father.MiddleName.or).WithMessage("The child's grandfather's name and his father's father's name do not match.").NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.LastName.am).Must(f => f == request.Father.MiddleName.am).WithMessage("The child's grandfather's name and his father's father's name do not match.").NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
            RuleFor(p => p.Event.EventOwener.BirthDate).NotEmpty().NotNull();
            // RuleFor(p => p.Event.EventOwener.PlaceOfBirthLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "PlaceOfBirthLookupId");
            RuleFor(p => p.Event.EventOwener.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "NationalityLookupId");
            RuleFor(p => p.Event.EventOwener.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "ResidentAddressId");


            // if (request.Father != null)
            // {
            // RuleFor(p => p.Father.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person, "Father.Id");
            RuleFor(p => p.Father.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.Father.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.Father.MiddleName.am).NotEmpty().NotNull();
            RuleFor(p => p.Father.MiddleName.or).NotEmpty().NotNull();
            RuleFor(p => p.Father.LastName.am).NotEmpty().NotNull();
            RuleFor(p => p.Father.LastName.or).NotEmpty().NotNull();
            RuleFor(p => p.Father.NationalId).NotGuidEmpty();
            RuleFor(p => p.Father.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.SexLookupId");
            RuleFor(p => p.Father.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.NationalityLookupId");
            RuleFor(p => p.Father.ReligionLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.ReligionLookupId");
            RuleFor(p => p.Father.EducationalStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.EducationalStatusLookupId");
            RuleFor(p => p.Father.TypeOfWorkLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.TypeOfWorkLookupId");
            RuleFor(p => p.Father.MarriageStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.MarriageStatusLookupId");
            RuleFor(p => p.Father.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "Father.ResidentAddressId");
            RuleFor(p => p.Father.NationLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.NationLookupId");
            RuleFor(p => p.Father.BirthDate).NotEmpty().NotNull()
                    .Must(date => date < DateTime.Now && date > new DateTime(1900, 1, 1));


            // }

            if (request.Event.EventRegistrar != null)
            {
                // RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person, "RegistrarInfo.Id");
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.LastName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Registrar.SexLookupId");
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "Registrar.ResidentAddressId");
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.BirthDate).NotEmpty().NotNull()
                    .Must(date => date < DateTime.Now && date > new DateTime(1900, 1, 1));
            }
            // if(request.Father == null && request.Mother == null)
            // {
            //     RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person);
            // }

            // if (request.Mother != null)
            // {
            // RuleFor(p => p.Mother.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person, "Mother.Id");
            RuleFor(p => p.Mother.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.Mother.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.Mother.MiddleName.am).NotEmpty().NotNull();
            RuleFor(p => p.Mother.MiddleName.or).NotEmpty().NotNull();
            RuleFor(p => p.Mother.LastName.am).NotEmpty().NotNull();
            RuleFor(p => p.Mother.LastName.or).NotEmpty().NotNull();
            RuleFor(p => p.Mother.NationalId).NotGuidEmpty();
            RuleFor(p => p.Mother.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Mother.SexLookupId");
            RuleFor(p => p.Mother.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Mother.NationalityLookupId");
            RuleFor(p => p.Mother.ReligionLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Mother.ReligionLookupId");
            RuleFor(p => p.Mother.EducationalStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Mother.EducationalStatusLookupId");
            RuleFor(p => p.Mother.TypeOfWorkLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Mother.TypeOfWorkLookupId");
            RuleFor(p => p.Mother.MarriageStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Mother.MarriageStatusLookupId");
            RuleFor(p => p.Mother.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "Mother.ResidentAddressId");
            RuleFor(p => p.Mother.NationLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Mother.NationLookupId");

            if (request.Event.EventSupportingDocuments != null)
            {
                RuleFor(p => p.Event.EventSupportingDocuments).SupportingDocNull("Event.EventSupportingDocuments").NotEmpty().NotNull();
            }

            if (request.Event.PaymentExamption != null)
            {
                RuleFor(p => p.Event.PaymentExamption.ExamptionRequestId.ToString()).NotGuidEmpty().ForeignKeyWithPaymentExamptionRequest(_repo.ExamptionRequest, "Event.PaymentExamption.ExamptionRequestId");
                RuleFor(p => p.Event.PaymentExamption.SupportingDocuments).SupportingDocNull("Event.PaymentExamption.EventSupportingDocuments").NotEmpty().NotNull();
            }
            // }
        }

    }
}