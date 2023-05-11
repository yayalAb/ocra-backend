
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
                    "Event.EventOwener.FirstName","Event.EventOwener.MiddleName","Event.EventOwener.LastName","Event.EventOwener.BirthDate",
                    "Event.EventOwener.NationalId","Event.EventOwener.SexLookupId","Event.EventOwener.PlaceOfBirthLookupId",
                    "Event.EventOwener.NationalityLookupId","Event.EventOwener.TitleLookupId","Event.EventOwener.ReligionLookupId",
                    "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
                    "Event.EventOwener.AddressId","Event.EventOwener.NationLookupId",
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
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.BirthAddressId)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.ResidentAddressId)).NotEmpty().NotNull();



        }
        private Expression<Func<T, object>> GetNestedProperty<T>(string propertyPath)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression body = param;
            foreach (var member in propertyPath.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);


            }
            if (Nullable.GetUnderlyingType(body.Type) != null)
            {
                // If the type is a nullable value type, convert it to its underlying type before converting to object
                body = Expression.Convert(body, Nullable.GetUnderlyingType(body.Type));
            }

            // Convert the result to object
            body = Expression.Convert(body, typeof(object));
            return Expression.Lambda<Func<T, object>>(body, param);
        }




    }
}