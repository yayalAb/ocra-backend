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

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    public class CreateBirthEventCommandValidator : AbstractValidator<CreateBirthEventCommand>
    {
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person) _repo;
        // private readonly IMediator _mediator;
        // private readonly ILogger<CreateBirthEventCommandValidator> log;
        public CreateBirthEventCommandValidator((ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person) repo, CreateBirthEventCommand request
        // , ILogger<CreateBirthEventCommandValidator> log
        )
        {

            // var predicateList = new List<Expression<Func<CreateBirthEventCommand, object>>>()
            // {
            //     (p => p.BirthEvent.FacilityLookupId),
            //     (p => p.BirthEvent.FacilityTypeLookupId),
            //     (p => p.BirthEvent.BirthPlaceId),
            //     (p => p.BirthEvent.TypeOfBirthLookupId),
            //     (p => p.BirthEvent.BirthNotification.DeliveryTypeLookupId),
            //     (p => p.BirthEvent.BirthNotification.SkilledProfLookupId),
            //     (p => p.BirthEvent.BirthNotification.WeightAtBirth),
            //     (p => p.BirthEvent.Event.EventOwener.FirstName.or),
            //     (p => p.BirthEvent.Event.EventOwener.FirstName.am),
            //     (p => p.BirthEvent.Event.EventOwener.SexLookupId),
            //     (p => p.BirthEvent.Event.EventOwener.BirthDate),
            //     (p => p.BirthEvent.Event.EventOwener.PlaceOfBirthLookupId),
            //     (p => p.BirthEvent.Event.EventOwener.NationalityLookupId),
            //     (p => p.BirthEvent.Event.EventOwener.ResidentAddressId),


            //     (p => p.BirthEvent.Father.FirstName.or),
            //     (p => p.BirthEvent.Father.FirstName.am),
            //     (p => p.BirthEvent.Father.MiddleName.am),
            //     (p => p.BirthEvent.Father.MiddleName.or),
            //     (p => p.BirthEvent.Father.LastName.am),
            //     (p => p.BirthEvent.Father.LastName.or),

            //     (p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.FirstName.or),
            //     (p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.FirstName.am),
            //     (p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.MiddleName.or),
            //     (p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.MiddleName.am),
            //     (p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.LastName.or),
            //     (p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.LastName.am),
            //     (p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.SexLookupId),
            //     (p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.ResidentAddressId),
            //     (p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.BirthDate),

            //     (p => p.BirthEvent.Mother.FirstName.or),
            //     (p => p.BirthEvent.Mother.FirstName.am),
            //     (p => p.BirthEvent.Mother.MiddleName.am),
            //     (p => p.BirthEvent.Mother.MiddleName.or),
            //     (p => p.BirthEvent.Mother.LastName.am),
            //     (p => p.BirthEvent.Mother.LastName.or),

            // };
            _repo = repo;

            RuleFor(p => p.BirthEvent.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityLookupId");
            RuleFor(p => p.BirthEvent.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityTypeLookupId");
            RuleFor(p => p.BirthEvent.BirthPlaceId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "BirthPlaceId");
            RuleFor(p => p.BirthEvent.TypeOfBirthLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "TypeOfBirthLookupId");
            RuleFor(p => p.BirthEvent.BirthNotification.DeliveryTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "DeliveryTypeLookupId");
            RuleFor(p => p.BirthEvent.BirthNotification.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SkilledProfLookupId");
            RuleFor(p => p.BirthEvent.BirthNotification.WeightAtBirth).NotEmpty().NotNull();
            RuleFor(p => p.BirthEvent.Event.EventOwener.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.BirthEvent.Event.EventOwener.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.BirthEvent.Event.EventOwener.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
            RuleFor(p => p.BirthEvent.Event.EventOwener.BirthDate).NotEmpty().NotNull();
            RuleFor(p => p.BirthEvent.Event.EventOwener.PlaceOfBirthLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "PlaceOfBirthLookupId");
            RuleFor(p => p.BirthEvent.Event.EventOwener.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "NationalityLookupId");
            RuleFor(p => p.BirthEvent.Event.EventOwener.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "ResidentAddressId");

            // if (request.BirthEvent.Father != null)
            // {
            // RuleFor(p => p.BirthEvent.Father.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person, "Father.Id");
            RuleFor(p => p.BirthEvent.Father.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Father.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Father.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Father.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Father.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Father.LastName.or).NotEmpty().NotNull();


            // }

            if (request.BirthEvent.Event.EventRegistrar != null)
            {
                // RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person, "RegistrarInfo.Id");
                RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.LastName.or).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
                RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "ResidentAddressId");
                RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.BirthDate).NotEmpty().NotNull()
                    .Must(date => date < DateTime.Now && date > new DateTime(1900, 1, 1));
            }
            // if(request.BirthEvent.Father == null && request.BirthEvent.Mother == null)
            // {
            //     RuleFor(p => p.BirthEvent.Event.EventRegistrar.RegistrarInfo.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person);
            // }

            // if (request.BirthEvent.Mother != null)
            // {
            // RuleFor(p => p.BirthEvent.Mother.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person, "Mother.Id");
            RuleFor(p => p.BirthEvent.Mother.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Mother.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Mother.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Mother.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Mother.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.BirthEvent.Mother.LastName.or).NotEmpty().NotNull();
            // }
        }

    }
}