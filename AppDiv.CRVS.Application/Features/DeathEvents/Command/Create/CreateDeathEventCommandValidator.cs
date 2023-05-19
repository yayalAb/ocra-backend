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

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Create
{
    public class CreateDeathEventCommandValidator : AbstractValidator<CreateDeathEventCommand>
    {
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person) _repo;
        // private readonly IMediator _mediator;
        public CreateDeathEventCommandValidator((ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person) repo, CreateDeathEventCommand request)
        {
            _repo = repo;
            RuleFor(p => p.DeathEvent.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityLookupId");
            RuleFor(p => p.DeathEvent.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityTypeLookupId");
            // RuleFor(p => p.DeathEvent.BirthPlaceId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo);
            // RuleFor(p => p.DeathEvent.DuringDeath).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.BirthCertificateId).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.PlaceOfFuneral).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.DeathNotification.CauseOfDeathInfoTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "CauseOfDeathInfoTypeLookupId");
            // RuleFor(p => p.DeathNotification.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.DeathEvent.DeathNotification.CauseOfDeath).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.DeathNotification.CauseOfDeathInfoTypeLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo.Lookup, "CauseOfDeathInfoTypeLookupId");
            RuleFor(p => p.DeathEvent.DeathNotification.DeathNotificationSerialNumber).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.Event.RegBookNo).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.Event.CivilRegOfficeCode).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.Event.CertificateId).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.Event.EventRegDate).NotEmpty().NotNull().Must(date => date > new DateTime(1900, 1, 1));
            RuleFor(p => p.DeathEvent.Event.CivilRegOfficerId.ToString()).NotEmpty().NotNull().ForeignKeyWithPerson(_repo.Person, "CivilRegOfficerId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.Event.EventOwener.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.Event.EventOwener.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.TitleLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "TitleLookupId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "NationalityLookupId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "ResidentAddressId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.ReligionLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "ReligionLookupId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.EducationalStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "EducationalStatusLookupId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.TypeOfWorkLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "TypeOfWorkLookupId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.MarriageStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "MarriageStatusLookupId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.NationLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "NationLookupId");


            if (request.DeathEvent.Event.EventRegistrar != null)
            {
                // RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person);
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.LastName.or).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "RegistrarInfo.SexLookupId");
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "RegistrarInfo.ResidentAddressId");
                // RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.BirthDate).NotEmpty().NotNull()
                // .Must(date => date < DateTime.Now && date > necw DateTime(1900, 1, 1));
            }
            else if (request.DeathEvent.Event.EventRegistrar == null)
            {
                RuleFor(p => p.DeathEvent.Event.EventRegistrar).Must(r => !(r == null)).WithMessage("Registrar Is Required");
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