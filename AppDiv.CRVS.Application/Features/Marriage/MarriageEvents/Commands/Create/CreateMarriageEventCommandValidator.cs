
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using FluentValidation;
using System.Linq.Expressions;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create
{
    public class CreateMarriageEventCommandValidator : AbstractValidator<CreateMarriageEventCommand>
    {
        private readonly ILookupRepository _lookupRepo;

        [Obsolete]
        public CreateMarriageEventCommandValidator(ILookupRepository lookupRepo)
        {
            _lookupRepo = lookupRepo;

            var fieldNames =
            new List<string>{

                "MarriageTypeId","ApplicationId","BrideInfo",
                    "BrideInfo.FirstName","BrideInfo.MiddleName","BrideInfo.LastName","BrideInfo.BirthDate",
                    "BrideInfo.NationalId","BrideInfo.SexLookupId","BrideInfo.PlaceOfBirthLookupId",
                    "BrideInfo.NationalityLookupId","BrideInfo.ReligionLookupId",
                    "BrideInfo.EducationalStatusLookupId","BrideInfo.TypeOfWorkLookupId","BrideInfo.MarriageStatusLookupId",
                    "BrideInfo.BirthAddressId","BrideInfo.NationLookupId","Event","Event.CertificateId", "Event.EventDate",
                    "Event.EventRegDate","Event.EventAddressId","Event.InformantTypeLookupId","Event.CivilRegOfficerId","Event.IsExampted",
                    "Event.EventOwener.FirstName","Event.EventOwener.MiddleName","Event.EventOwener.LastName","Event.EventOwener.BirthDate",
                    "Event.EventOwener.NationalId","Event.EventOwener.SexLookupId","Event.EventOwener.PlaceOfBirthLookupId",
                    "Event.EventOwener.NationalityLookupId","Event.EventOwener.ReligionLookupId",
                    "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
                    "Event.EventOwener.ResidentAddressId","Event.EventOwener.BirthAddressId","Event.EventOwener.NationLookupId",
                    "Event.EventRegistrar.RelationshipLookupId",

            };
            var eventRegistrarFeilds = new List<string>{
                "Event.EventRegistrar.RegistrarInfo.FirstName","Event.EventRegistrar.RegistrarInfo.MiddleName","Event.EventRegistrar.RegistrarInfo.LastName","Event.EventRegistrar.RegistrarInfo.BirthDate",
                    "Event.EventRegistrar.RegistrarInfo.NationalId","Event.EventRegistrar.RegistrarInfo.SexLookupId","Event.EventRegistrar.RegistrarInfo.PlaceOfBirthLookupId",
                    "Event.EventRegistrar.RegistrarInfo.NationalityLookupId","Event.EventRegistrar.RegistrarInfo.ReligionLookupId",
                    "Event.EventRegistrar.RegistrarInfo.EducationalStatusLookupId","Event.EventRegistrar.RegistrarInfo.TypeOfWorkLookupId","Event.EventRegistrar.RegistrarInfo.MarriageStatusLookupId",
                    "Event.EventRegistrar.RegistrarInfo.ResidentAddressId","Event.EventRegistrar.RegistrarInfo.BirthAddressId","Event.EventRegistrar.RegistrarInfo.NationLookupId",

            };
            foreach (var fieldName in fieldNames)
            {
                var rule = RuleFor(GetNestedProperty<CreateMarriageEventCommand>(fieldName))
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .WithMessage("{PropertyName} must not be null.")
                    .NotEmpty()
                    .WithMessage("{PropertyName} must not be empty.");

                // add more validation rules for the field here, if needed
            }
            When(e => e.Event.EventRegistrar != null, () =>
            {
                foreach (var feildName in eventRegistrarFeilds)
                {

                    var rule2 = RuleFor(GetNestedProperty<CreateMarriageEventCommand>(feildName))
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull()
                        .WithMessage("{PropertyName} must not be null.")
                        .NotEmpty()
                        .WithMessage("{PropertyName} must not be empty.");
                }
            });

            RuleFor(e => e.Witnesses)
            .NotNull()
            .Must(w => w.Count >= 4).WithMessage("there should be atleast 4 witnesses to register a devorce");

            RuleFor(e => e.Witnesses.Select(w => w.WitnessFor)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.FirstName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.MiddleName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.LastName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.SexLookupId)).NotEmpty().NotNull();
            //only resident address is required
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.ResidentAddressId)).NotEmpty().NotNull();

            RuleFor(e => e.BrideInfo.BirthDate)
            .Must(BeAbove18YearsOld).WithMessage("the bride cannot be below 18 years old");
            RuleFor(e => e.Event.EventOwener.BirthDate)
            .Must(BeAbove18YearsOld).WithMessage("the Groom cannot be below 18 years old");

            When(e => isDivorcee(e.BrideInfo.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .Must(haveDevorceCertificateAttachement).WithMessage("divorce paper document should be attached if bride is a divorcee");
            });
            When(e => isDivorcee(e.Event.EventOwener.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .Must(haveDevorceCertificateAttachement).WithMessage("divorce paper document should be attached if eventOwner(Groom) is a divorcee");
            });
            When(e => isWidowed(e.Event.EventOwener.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .Must(haveDeathCertificateAttachement).WithMessage("Death Certificate document should be attached if eventOwner(Groom) is a Widowed");
            });
             When(e => isDivorcee(e.BrideInfo.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .Must(haveDeathCertificateAttachement).WithMessage("death certificate paper document should be attached if bride is a divorcee");
            });
        }


        private bool haveDevorceCertificateAttachement(ICollection<AddSupportingDocumentRequest>? supportingDocs)
        {
            return supportingDocs == null || supportingDocs.ToList()
             .Where(doc => doc.Type.ToLower() == Enum.GetName<SupportingDcoumentType>(SupportingDcoumentType.DivorcePaper)!.ToLower())
             .Any();
        }
        private bool haveDeathCertificateAttachement(ICollection<AddSupportingDocumentRequest>? supportingDocs)
        {
            return supportingDocs == null || supportingDocs.ToList()
             .Where(doc => doc.Type.ToLower() == Enum.GetName<SupportingDcoumentType>(SupportingDcoumentType.DeathCertificate)!.ToLower())
             .Any();
        }


        private bool isDivorcee(Guid marriageStatusLookupId)
        {
            var marriageStatus = _lookupRepo.GetLookupById(marriageStatusLookupId);
            if (marriageStatus == null)
            {
                return false;
            }
            return marriageStatus.ValueStr.Contains(Enum.GetName<MarriageStatus>(MarriageStatus.divorced)!.ToLower());
        }
        private bool isWidowed(Guid marriageStatusLookupId)
        {
            var marriageStatus = _lookupRepo.GetLookupById(marriageStatusLookupId);
            if (marriageStatus == null)
            {
                return false;
            }
            return marriageStatus.ValueStr.Contains(Enum.GetName<MarriageStatus>(MarriageStatus.widowed)!.ToLower());
        }

        private bool BeAbove18YearsOld(DateTime birthDate)
        {
            return DateTime.Now.Year - birthDate.Year >= 18;
        }

        private bool fds(object arg)
        {
            throw new NotImplementedException();
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