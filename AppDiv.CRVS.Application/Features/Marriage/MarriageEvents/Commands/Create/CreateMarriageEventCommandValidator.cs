
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create
{
    public class CreateMarriageEventCommandValidator : AbstractValidator<CreateMarriageEventCommand>
    {
        private readonly ILookupRepository _lookupRepo;
        private readonly IMarriageApplicationRepository _marriageApplicationRepo;
        private readonly IPersonalInfoRepository _personalInfoRepo;
        private readonly IDivorceEventRepository _divorceEventRepo;

        [Obsolete]
        public CreateMarriageEventCommandValidator(ILookupRepository lookupRepo, IMarriageApplicationRepository marriageApplicationRepo, IPersonalInfoRepository personalInfoRepo, IDivorceEventRepository divorceEventRepo)
        {
            _lookupRepo = lookupRepo;
            _marriageApplicationRepo = marriageApplicationRepo;
            _personalInfoRepo = personalInfoRepo;
            _divorceEventRepo = divorceEventRepo;
            var fieldNames =
            new List<string>{

                "MarriageTypeId","ApplicationId","BrideInfo",
                    "BrideInfo.FirstName","BrideInfo.MiddleName","BrideInfo.LastName","BrideInfo.BirthDate",
                    "BrideInfo.NationalId","BrideInfo.SexLookupId","BrideInfo.PlaceOfBirthLookupId",
                    "BrideInfo.NationalityLookupId","BrideInfo.ReligionLookupId",
                    "BrideInfo.EducationalStatusLookupId","BrideInfo.TypeOfWorkLookupId","BrideInfo.MarriageStatusLookupId",
                    "BrideInfo.BirthAddressId","BrideInfo.NationLookupId","Event","Event.CertificateId", "Event.EventDate",
                    "Event.EventRegDate","Event.EventAddressId","Event.CivilRegOfficerId","Event.IsExampted",
                    "Event.EventRegDate","Event.EventAddressId","Event.CivilRegOfficerId","Event.IsExampted",
                    "Event.EventOwener.FirstName","Event.EventOwener.MiddleName","Event.EventOwener.LastName","Event.EventOwener.BirthDate",
                    "Event.EventOwener.NationalId","Event.EventOwener.SexLookupId","Event.EventOwener.PlaceOfBirthLookupId",
                    "Event.EventOwener.NationalityLookupId","Event.EventOwener.ReligionLookupId",
                    "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
                    "Event.EventOwener.ResidentAddressId","Event.EventOwener.BirthAddressId","Event.EventOwener.NationLookupId",

            };
            var lookupFeilds = new List<string>{
               "MarriageTypeId","BrideInfo.SexLookupId","BrideInfo.PlaceOfBirthLookupId",
                    "BrideInfo.NationalityLookupId","BrideInfo.ReligionLookupId",
                    "BrideInfo.EducationalStatusLookupId","BrideInfo.TypeOfWorkLookupId","BrideInfo.MarriageStatusLookupId",
                    "BrideInfo.NationLookupId","Event.InformantTypeLookupId","Event.EventOwener.SexLookupId","Event.EventOwener.PlaceOfBirthLookupId",
                    "Event.EventOwener.NationalityLookupId","Event.EventOwener.ReligionLookupId",
                    "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
                    "Event.EventOwener.NationLookupId"
            };
            foreach (var lookupFeild in lookupFeilds)
            {
                var rule = RuleFor(GetNestedProperty<CreateMarriageEventCommand>(lookupFeild))
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .Must(BeFoundInLookupTable)
                    .WithMessage("{PropertyName} with the provided id is not found");

                // add more validation rules for the field here, if needed
            }
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
            RuleFor(e => e.Event.CivilRegOfficerId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("civilRegOfficerId cannot be null")
                .NotEmpty().WithMessage("civilRegOfficerId cannot be empty")
                .Must(BeFoundInPersonalInfoTable).WithMessage("civilRegistrar officer with the provided id is not found");



            RuleFor(e => e.Witnesses)
            .NotNull()
            .Must(w => w.Count >= 4).WithMessage("there should be atleast 4 witnesses to register a divorce");

            RuleFor(e => e.Witnesses.Select(w => w.WitnessFor)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.FirstName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.MiddleName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.LastName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.SexLookupId))
            .ForEach(lookupId => lookupId
                    .NotEmpty()
                    .NotNull()
                    .Must(BeFoundInLookupTable).WithMessage("witness sexLookup with the provided id is not found"));

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
                .MustAsync(async (model, supportingDocs, CancellationToken) => await haveDevorceCertificateAttachementAsync(supportingDocs, model.BrideInfo.Id, "wife")).WithMessage("divorce paper document should be attached if bride is a divorcee");
            });
            When(e => isDivorcee(e.Event.EventOwener.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .MustAsync(async (model, supportingDocs, CancellationToken) => await haveDevorceCertificateAttachementAsync(supportingDocs, model.Event.EventOwener.Id, "husband")).WithMessage("divorce paper document should be attached if eventOwner(Groom) is a divorcee");
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
            When(e => isCivilMarriage(e.MarriageTypeId), () =>
            {
                RuleFor(e => e.ApplicationId)
                .NotNull().WithMessage("marriage application id is required for 'civil' marriage type")
                .NotEmpty().WithMessage("marriage application id cannot be empty for 'civil' marriage type")
                .Must(BeFoundInMarriageApplicationTable).WithMessage("marriage application with the provided id not found");

                RuleFor(e => e.Event.EventRegDate)
                .MustAsync(async (model, eventRegDate, CancellationToken) => await Be30DaysAfterMarriageApplicationDateAsync(eventRegDate, model))
                .WithMessage("there should be atleast 30 day gap between marriage application date and marriage registered date");
            });
            When(e => e.Event.IsExampted, () =>
            {
                RuleFor(e => e.Event.PaymentExamption)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty().WithMessage("payment Examption cannot be empty if isExapmted = true")
                    .NotNull().WithMessage("payment Examption cannot be null if isExapmted = true");

            });
        }

        private bool BeFoundInPersonalInfoTable(Guid guid)
        {
            return _personalInfoRepo.GetById(guid) != null;
        }

        private bool BeFoundInLookupTable(object lookupId)
        {
            var l = lookupId;
            // return false;

            return lookupId != null && _lookupRepo.GetLookupById((Guid)lookupId) != null;
        }
        private bool BeFoundInLookupTable(Guid lookupId)
        {
            return lookupId != null && _lookupRepo.GetLookupById((Guid)lookupId) != null;
        }


        private async Task<bool> Be30DaysAfterMarriageApplicationDateAsync(DateTime marriageRegDate, CreateMarriageEventCommand marriageEvent)
        {
            var application = await _marriageApplicationRepo.GetAsync(marriageEvent.ApplicationId!);


            return (marriageRegDate - application.ApplicationDate).Days >= 30;
        }

        private bool BeFoundInMarriageApplicationTable(Guid? applicationId)
        {
            return applicationId != null && _marriageApplicationRepo.exists((Guid)applicationId);
        }

        private bool isCivilMarriage(Guid marriageTypeId)
        {
            var marriageType = _lookupRepo.GetLookupById(marriageTypeId);
            if (marriageType == null)
            {
                return false;
            }
            // marriageType.ValueStr.Contains(Enum.GetName<MarriageType>(MarriageType.Civil)!.ToLower())
            return marriageType.Value.Value<string>("en")?.ToLower() == Enum.GetName<MarriageType>(MarriageType.Civil)!.ToLower()
             || marriageType.Value.Value<string>("am")?.ToLower() == Enum.GetName<MarriageType>(MarriageType.Civil)!.ToLower()
             || marriageType.Value.Value<string>("or")?.ToLower() == Enum.GetName<MarriageType>(MarriageType.Civil)!.ToLower();


        }

        private async Task<bool> haveDevorceCertificateAttachementAsync(ICollection<AddSupportingDocumentRequest>? supportingDocs, Guid? perosnalInfoId, string type)
        {
            bool hasRegisteredDivorceCertificate = false;
            if (type == "wife")
            {

                hasRegisteredDivorceCertificate = perosnalInfoId != null
                         && await  _divorceEventRepo.GetAllQueryableAsync().Where(e => e.DivorcedWifeId == perosnalInfoId).AnyAsync();
            }
            else
            {
                hasRegisteredDivorceCertificate = perosnalInfoId != null
                         && await _divorceEventRepo.GetAllQueryableAsync().Where(e => e.Event.EventOwenerId == perosnalInfoId).AnyAsync();
            }
            return hasRegisteredDivorceCertificate || supportingDocs == null || supportingDocs.ToList()
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