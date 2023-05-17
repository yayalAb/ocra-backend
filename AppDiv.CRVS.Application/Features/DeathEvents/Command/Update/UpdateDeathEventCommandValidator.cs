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
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person) _repo;
        // private readonly IMediator _mediator;
        public UpdateDeathEventCommandValidator((ILookupRepository Lookup, IAddressLookupRepository Address, IPersonalInfoRepository Person) repo, UpdateDeathEventCommand request)
        {
            _repo = repo;
            RuleFor(p => p.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityLookupId");
            RuleFor(p => p.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityTypeLookupId");
            // RuleFor(p => p.BirthPlaceId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo);
            RuleFor(p => p.DuringDeath).NotEmpty().NotNull();
            RuleFor(p => p.PlaceOfFuneral).NotEmpty().NotNull();
            RuleFor(p => p.DeathNotification.CauseOfDeathInfoTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "CauseOfDeathInfoTypeLookupId");
            // RuleFor(p => p.DeathNotification.SkilledProfLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo);
            RuleFor(p => p.DeathNotification.CauseOfDeath).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
            // RuleFor(p => p.Event.EventOwener.BirthDate).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventOwener.PlaceOfBirthLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "PlaceOfBirthLookupId");
            RuleFor(p => p.Event.EventOwener.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "NationalityLookupId");
            RuleFor(p => p.Event.EventOwener.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "ResidentAddressId");


            if (request.Event.EventRegistrar != null)
            {
                // RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo.Person);
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.FirstName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.FirstName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.MiddleName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.MiddleName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.LastName.or).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.LastName.am).NotEmpty().NotNull();
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
                RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "ResidentAddressId");
                // RuleFor(p => p.Event.EventRegistrar.RegistrarInfo.BirthDate).NotEmpty().NotNull()
                // .Must(date => date < DateTime.Now && date > necw DateTime(1900, 1, 1));
            }
            else if (request.Event.EventRegistrar == null)
            {
                RuleFor(p => p.Event.EventRegistrar).Must(r => !(r == null)).WithMessage("Registrar Is Required");
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