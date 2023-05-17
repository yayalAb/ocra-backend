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
            RuleFor(p => p.DeathEvent.DuringDeath).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.PlaceOfFuneral).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.DeathNotification.CauseOfDeathInfoTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "CauseOfDeathInfoTypeLookupId");
            // RuleFor(p => p.DeathEvent.DeathNotification.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.DeathEvent.DeathNotification.CauseOfDeath).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.Event.EventOwener.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.Event.EventOwener.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.Event.EventOwener.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
            // RuleFor(p => p.DeathEvent.Event.EventOwener.BirthDate).NotEmpty().NotNull();
            RuleFor(p => p.DeathEvent.Event.EventOwener.PlaceOfBirthLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "PlaceOfBirthLookupId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "NationalityLookupId");
            RuleFor(p => p.DeathEvent.Event.EventOwener.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "ResidentAddressId");


            if (request.DeathEvent.Event.EventRegistrar != null)
            {
                // RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person);
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.LastName.or).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
                RuleFor(p => p.DeathEvent.Event.EventRegistrar.RegistrarInfo.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "ResidentAddressId");
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