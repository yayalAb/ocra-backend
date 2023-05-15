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
        private readonly IEventRepository _repo;
        // private readonly IMediator _mediator;
        // private readonly ILogger<UpdateBirthEventCommandValidator> log;
        public UpdateBirthEventCommandValidator(IEventRepository repo, UpdateBirthEventCommand request
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

            RuleFor(p => p.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.BirthPlaceId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo);
            RuleFor(p => p.TypeOfBirthLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.BirthNotification.DeliveryTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.BirthNotification.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.BirthNotification.WeightAtBirth).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.Event.EventOwener.BirthDate).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.PlaceOfBirthLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.Event.EventOwener.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.Event.EventOwener.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);

            if (string.IsNullOrEmpty(request.FatherId.ToString()) && request.Father != null)
            {
                RuleFor(p => p.Father.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo);
                RuleFor(p => p.Father.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.Father.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.Father.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.Father.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.Father.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.Father.LastName.or).NotEmpty().NotNull();

            }
            else
            {
                RuleFor(p => p.FatherId.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo);
            }
            if (!string.IsNullOrEmpty(request.Event.EventRegistrar?.RegistrarInfoId.ToString()) && request.Event.EventRegistrar != null)
            {
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo);
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.LastName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.BirthDate).NotEmpty().NotNull()
                    .Must(date => date < DateTime.Now && date > new DateTime(1900, 1, 1));
            }
            else
            {
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfoId.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo);
            }

            if (!string.IsNullOrEmpty(request.MotherId.ToString()) && request.Mother != null)
            {
                RuleFor(p => p.Mother.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo);
                RuleFor(p => p.Mother.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.Mother.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.Mother.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.Mother.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.Mother.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.Mother.LastName.or).NotEmpty().NotNull();
            }
            else
            {
                RuleFor(p => p.MotherId.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo);
            }
        }

    }
}