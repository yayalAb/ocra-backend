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
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person) _repo;
        // private readonly IMediator _mediator;
        // private readonly ILogger<UpdateBirthEventCommandValidator> log;
        public UpdateBirthEventCommandValidator((ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person) repo, UpdateBirthEventCommand request
        // , ILogger<CreateBirthEventCommandValidator> log
        )
        {

            // var predicateList = new List<Expression<Func<CreateBirthEventCommand, object>>>()
            // {
            //     (p => p.FacilityLookupId),
            //     (p => p.FacilityTypeLookupId),
            //     (p => p.BirthPlaceId),
            //     (p => p.TypeOfBirthLookupId),
            //     (p => p.BirthNotification.DeliveryTypeLookupId),
            //     (p => p.BirthNotification.SkilledProfLookupId),
            //     (p => p.BirthNotification.WeightAtBirth),
            //     (p => p.Event.EventOwener.FirstName.or),
            //     (p => p.Event.EventOwener.FirstName.am),
            //     (p => p.Event.EventOwener.SexLookupId),
            //     (p => p.Event.EventOwener.BirthDate),
            //     (p => p.Event.EventOwener.PlaceOfBirthLookupId),
            //     (p => p.Event.EventOwener.NationalityLookupId),
            //     (p => p.Event.EventOwener.ResidentAddressId),


            //     (p => p.Father.FirstName.or),
            //     (p => p.Father.FirstName.am),
            //     (p => p.Father.MiddleName.am),
            //     (p => p.Father.MiddleName.or),
            //     (p => p.Father.LastName.am),
            //     (p => p.Father.LastName.or),

            //     (p => p.Event.EventRegistrar.RegistrarInfo.FirstName.or),
            //     (p => p.Event.EventRegistrar.RegistrarInfo.FirstName.am),
            //     (p => p.Event.EventRegistrar.RegistrarInfo.MiddleName.or),
            //     (p => p.Event.EventRegistrar.RegistrarInfo.MiddleName.am),
            //     (p => p.Event.EventRegistrar.RegistrarInfo.LastName.or),
            //     (p => p.Event.EventRegistrar.RegistrarInfo.LastName.am),
            //     (p => p.Event.EventRegistrar.RegistrarInfo.SexLookupId),
            //     (p => p.Event.EventRegistrar.RegistrarInfo.ResidentAddressId),
            //     (p => p.Event.EventRegistrar.RegistrarInfo.BirthDate),

            //     (p => p.Mother.FirstName.or),
            //     (p => p.Mother.FirstName.am),
            //     (p => p.Mother.MiddleName.am),
            //     (p => p.Mother.MiddleName.or),
            //     (p => p.Mother.LastName.am),
            //     (p => p.Mother.LastName.or),

            // };
            _repo = repo;

            RuleFor(p => p.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityLookupId");
            RuleFor(p => p.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityTypeLookupId");
            RuleFor(p => p.BirthPlaceId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "BirthPlaceId");
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
            // }
        }

    }
}