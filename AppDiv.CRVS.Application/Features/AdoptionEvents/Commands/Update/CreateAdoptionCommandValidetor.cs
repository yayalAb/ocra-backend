using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update
{
    public class CreateAdoptionCommandValidetor : AbstractValidator<UpdateAdoptionCommand>
    {
        private readonly IAdoptionEventRepository _repo;
        public CreateAdoptionCommandValidetor(IAdoptionEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(e => e.AdoptiveFather)
           .Must((e, adoptiveFather) => adoptiveFather != null || e.AdoptiveMother != null).WithMessage("both adoptive mother and father cannot be null when registering adoption");
            When(e => e.AdoptiveFather != null, () =>
            {
                RuleFor(e => e.Event.EventOwener.MiddleName)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .NotEmpty()
                    .Must((e, childMiddleName) => BeEqual(childMiddleName, e.AdoptiveFather!.FirstName)).WithMessage("the Child's MiddleName must be same as the Adoptive Father's name");
                RuleFor(e => e.Event.EventOwener.LastName)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .NotEmpty()
                    .Must((e, childLastName) => BeEqual(childLastName, e.AdoptiveFather!.MiddleName)).WithMessage("the Child's lastname must be same as the Adoptive Father's MiddleName");

            });
            When(e => e.AdoptiveFather == null && e.AdoptiveMother != null, () =>
            {
                RuleFor(e => e.Event.EventOwener.MiddleName)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .NotEmpty()
                    .Must((e, childMiddleName) => BeEqual(childMiddleName, e.AdoptiveMother!.MiddleName)).WithMessage("the Child's MiddleName must be same as the Adoptive Mother's MiddleName");
                RuleFor(e => e.Event.EventOwener.LastName)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .NotEmpty()
                    .Must((e, childLastName) => BeEqual(childLastName, e.AdoptiveMother!.LastName)).WithMessage("the Child's lastname must be same as the Adoptive Mother's Lastname");

            });
            RuleFor(p => p.ApprovedName.am)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.ApprovedName.or)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.AdoptiveFather.FirstName.am)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.AdoptiveFather.FirstName.or)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.AdoptiveFather.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.AdoptiveMother.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.Event.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            // RuleFor(p => p.Event.PaymentExamption.Id)
            //     .Cascade(CascadeMode.StopOnFirstFailure)
            //     .NotNull()
            //     .WithMessage("{PropertyName} is required.")
            //     .NotEmpty()
            //     .WithMessage("{PropertyName} must not be empty.");
        }
        private bool BeEqual(LanguageModel obj1, LanguageModel obj2)
        {
            return obj1.am == obj2.am && obj1.en == obj2.en && obj1.or == obj2.or;
        }
    }
}

