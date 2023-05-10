
using FluentValidation;
using System.Linq.Expressions;


namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create
{
    public class CreateMarriageEventCommandValidator : AbstractValidator<CreateMarriageEventCommand>
    {
        public CreateMarriageEventCommandValidator()
        {
            var fieldNames = new List<string>{"MarriageTypeId","ApplicationId","BrideInfo",
                    "BrideInfo.FirstName","BrideInfo.MiddleName","BrideInfo.LastName","BrideInfo.BirthDate",
                    "BrideInfo.NationalId","BrideInfo.SexLookupId","BrideInfo.PlaceOfBirthLookupId",
                    "BrideInfo.NationalityLookupId","BrideInfo.TitleLookupId","BrideInfo.ReligionLookupId",
                    "BrideInfo.EducationalStatusLookupId","BrideInfo.TypeOfWorkLookupId","BrideInfo.MarriageStatusLookupId",
                    "BrideInfo.AddressId","BrideInfo.NationLookupId","Event",
                    "Event.EventOwner.FirstName","Event.EventOwner.MiddleName","Event.EventOwner.LastName","Event.EventOwner.BirthDate",
                    "Event.EventOwner.NationalId","Event.EventOwner.SexLookupId","Event.EventOwner.PlaceOfBirthLookupId",
                    "Event.EventOwner.NationalityLookupId","Event.EventOwner.TitleLookupId","Event.EventOwner.ReligionLookupId",
                    "Event.EventOwner.EducationalStatusLookupId","Event.EventOwner.TypeOfWorkLookupId","Event.EventOwner.MarriageStatusLookupId",
                    "Event.EventOwner.AddressId","Event.EventOwner.NationLookupId",
                    "Event.EventRegistrar.RelationshipId",
                    "Event.EventRegistrar.RegistrarInfo.FirstName","Event.EventRegistrar.RegistrarInfo.MiddleName","Event.EventRegistrar.RegistrarInfo.LastName","Event.EventRegistrar.RegistrarInfo.BirthDate",
                    "Event.EventRegistrar.RegistrarInfo.NationalId","Event.EventRegistrar.RegistrarInfo.SexLookupId","Event.EventRegistrar.RegistrarInfo.PlaceOfBirthLookupId",
                    "Event.EventRegistrar.RegistrarInfo.NationalityLookupId","Event.EventRegistrar.RegistrarInfo.TitleLookupId","Event.EventRegistrar.RegistrarInfo.ReligionLookupId",
                    "Event.EventRegistrar.RegistrarInfo.EducationalStatusLookupId","Event.EventRegistrar.RegistrarInfo.TypeOfWorkLookupId","Event.EventRegistrar.RegistrarInfo.MarriageStatusLookupId",
                    "Event.EventRegistrar.RegistrarInfo.AddressId","Event.EventRegistrar.RegistrarInfo.NationLookupId",
            
            };
            foreach (var fieldName in fieldNames)
            {
                var rule = RuleFor(GetNestedProperty<CreateMarriageEventCommand>(fieldName))
                    .NotNull()
                    .WithMessage("{PropertyName} must not be null.")
                    .NotEmpty()
                    .WithMessage("{PropertyName} must not be empty.");

                // add more validation rules for the field here, if needed
            }
            RuleFor(e => e.Witnesses.Select(w => w.WitnessFor)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.FirstName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.MiddleName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.LastName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.SexLookupId)).NotEmpty().NotNull();
            //only resident address is required
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.AddressId)).NotEmpty().NotNull();


        }
        private Expression<Func<T, object>> GetNestedProperty<T>(string propertyPath)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression body = param;
            foreach (var member in propertyPath.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }
            return Expression.Lambda<Func<T, object>>(body, param);
        }
       



    }
}