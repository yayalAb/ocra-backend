using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, string?> NotStartWithWhiteSpace<T>(this IRuleBuilder<T, string?> ruleBuilder, string type)
        {

            return ruleBuilder.Must(m => m != null && !m.StartsWith(" ")).WithMessage("'{PropertyName}' should not start with whitespace");
        }

        public static IRuleBuilderOptions<T, string> NotEndWithWhiteSpace<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(m => m != null && !m.EndsWith(" ")).WithMessage("'{PropertyName}' should not end with whitespace");
        }
    }
    public class CreateBirthEventCommandValidator : AbstractValidator<CreateBirthEventCommand>
    {
        private readonly IBirthEventRepository _repo;
        // private readonly IMediator _mediator;
        // private readonly ILogger<CreateBirthEventCommandValidator> log;
        public CreateBirthEventCommandValidator(IBirthEventRepository repo
        // , ILogger<CreateBirthEventCommandValidator> log
        )
        {
            var props = new List<string>
            {
                "BirthEvent.FacilityLookupId", "BirthEvent.FacilityTypeLookupId", "BirthEvent.TypeOfBirthLookupId", "BirthEvent.Event.InformantTypeLookupId",
                "BirthEvent.Event.EventAddressId", "BirthEvent.Event.EventOwener.NationalityLookupId", "BirthEvent.Event.EventOwener.PlaceOfBirthLookupId",
                "BirthEvent.Event.EventOwener.BirthAddressId", "BirthEvent.Event.EventOwener.ResidentAddressId", "BirthEvent.Event.EventOwener.SexLookupId",
                "BirthEvent.Event.CivilRegOfficerId", "BirthEvent.Event.EventRegistrar.RelationshipLookupId"
            };
            // this.log = log;
            // RuleFor(p => p.BirthEvent.FacilityLookupId.ToString()).NotStartWithWhiteSpace("hgjh");
            _repo = repo;
            foreach (var prop in props)
            {
                var rule = RuleFor(ValidationService.GetNestedProperty<CreateBirthEventCommand>(prop))
                    .IsGuidNullOrEmpty().WithMessage("{PropertyName} must not be Empty Guid.")
                    .NotNull().WithMessage("{PropertyName} must not be null.")
                    .NotEmpty().WithMessage("{PropertyName} must not be empty.");
            }
            // RuleFor(p => p.BirthEvent.FacilityLookupId).NotEmpty().NotNull().NotGuidEmpty();
            // RuleFor(p => p.BirthEvent.FacilityTypeLookupId).NotEmpty().NotNull().NotGuidEmpty();
            RuleFor(p => p.BirthEvent.BirthPlaceId).NotEmpty().NotNull().NotGuidEmpty();
            RuleFor(p => p.BirthEvent.TypeOfBirthLookupId).NotEmpty().NotNull().NotGuidEmpty();
            RuleFor(p => p.BirthEvent.BirthNotification.DeliveryTypeLookupId).NotEmpty().NotNull().NotGuidEmpty();
            RuleFor(p => p.BirthEvent.BirthNotification.SkilledProfLookupId).NotEmpty().NotNull().NotGuidEmpty();
            RuleFor(p => p.BirthEvent.BirthNotification.WeightAtBirth).NotEmpty().NotNull();
            RuleFor(p => p.BirthEvent.Event.EventOwener.FirstName).NotEmpty().NotNull();

            // RuleFor(p => p.BirthEvent.BirthPlaceId)
            // .Must(ff);
            // _mediator = mediator;
            // RuleFor(p => p.BirthEvent.Event.EventOwener.FirstName)
            //     .Must(man).WithMessage("Payment Type must not be empty.");
            // .NotEmpty().WithMessage("{PropertyName} is required.")
            // .NotNull()
            // .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            // RuleFor(p => p.BirthEvent.Event.EventOwener.FirstName)
            //     // .Must(x => x != Guid.Empty).WithMessage("Payment Type must not be empty.");
            // .NotEmpty().WithMessage("{PropertyName} is required.")
            // .NotNull()
            // .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            // RuleFor(p => p.BirthEvent.EventLookupId)
            //     .Must(x => x != Guid.Empty).WithMessage("Event must not be empty.");

            // RuleFor(p => p.BirthEvent.FacilityLookupId)
            //     .Must(lookup).WithMessage("Address must not be empty.");
            // RuleFor(pr => pr.BirthEvent.Amount)
            //     .NotEmpty().WithMessage("{PropertyName} is required.")
            //     .NotNull();

            // RuleFor(e => e)
            //   .MustAsync(phoneNumberUnique)
            //   .WithMessage("A Customer phoneNumber already exists.");
        }

        private bool em(object arg)
        {
            throw new NotImplementedException();
        }

        private bool d(Guid? nullable)
        {
            throw new NotImplementedException();
        }

        private bool ff(CreateBirthEventCommand command, Guid? nullable, ValidationContext<CreateBirthEventCommand> context)
        {
            // log.LogCritical(context.PropertyName);
            // context.RootContextData
            return true;
        }

        private bool man(JObject @object)
        {
            throw new NotImplementedException();
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