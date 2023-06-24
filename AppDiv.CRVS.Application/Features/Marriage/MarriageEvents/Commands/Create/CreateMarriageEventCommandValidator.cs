
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Utility.Services;
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
        private readonly IMarriageEventRepository _marriageEventRepo;
        private readonly IPaymentExamptionRequestRepository _paymentExamptionRequestRepo;
        private readonly IAddressLookupRepository _addressRepo;
        private readonly IEventRepository _eventRepo;

        [Obsolete]
        public CreateMarriageEventCommandValidator(ILookupRepository lookupRepo, IMarriageApplicationRepository marriageApplicationRepo, IPersonalInfoRepository personalInfoRepo, IDivorceEventRepository divorceEventRepo, IMarriageEventRepository marriageEventRepo, IPaymentExamptionRequestRepository paymentExamptionRequestRepo, IAddressLookupRepository addressRepo, IEventRepository eventRepo)
        {
            _lookupRepo = lookupRepo;
            _marriageApplicationRepo = marriageApplicationRepo;
            _personalInfoRepo = personalInfoRepo;
            _divorceEventRepo = divorceEventRepo;
            _marriageEventRepo = marriageEventRepo;
            _paymentExamptionRequestRepo = paymentExamptionRequestRepo;
            _addressRepo = addressRepo;
            _eventRepo = eventRepo;


            var fieldNames =
            new List<string>{

                "MarriageTypeId","BrideInfo",
                    "BrideInfo.FirstName","BrideInfo.MiddleName","BrideInfo.LastName",
                    "BrideInfo.BirthDateEt",
                    "BrideInfo.NationalId",
                    "BrideInfo.NationalityLookupId","BrideInfo.ReligionLookupId","BrideInfo.ResidentAddressId",
                    "BrideInfo.EducationalStatusLookupId","BrideInfo.TypeOfWorkLookupId","BrideInfo.MarriageStatusLookupId",
                    "BrideInfo.BirthAddressId","BrideInfo.NationLookupId","Event.CertificateId", "Event.EventDateEt",
                    "Event.EventRegDateEt","Event.EventAddressId","Event.CivilRegOfficerId","Event.IsExampted",
                    "Event.EventOwener.FirstName","Event.EventOwener.MiddleName","Event.EventOwener.LastName","Event.EventOwener.BirthDateEt",
                    "Event.EventOwener.NationalId",
                    "Event.EventOwener.NationalityLookupId","Event.EventOwener.ReligionLookupId",
                    "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
                    "Event.EventOwener.ResidentAddressId","Event.EventOwener.BirthAddressId","Event.EventOwener.NationLookupId",

            };
            var lookupFeilds = new List<string>{
               "MarriageTypeId",
                    "BrideInfo.NationalityLookupId","BrideInfo.ReligionLookupId",
                    "BrideInfo.EducationalStatusLookupId","BrideInfo.TypeOfWorkLookupId","BrideInfo.MarriageStatusLookupId",
                    "BrideInfo.NationLookupId",
                    "Event.EventOwener.NationalityLookupId","Event.EventOwener.ReligionLookupId",
                    "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
                    "Event.EventOwener.NationLookupId"
            };
            foreach (var fieldName in fieldNames)
            {
                var rule = RuleFor(GetNestedProperty<CreateMarriageEventCommand>(fieldName))
                    .NotNull()
                    .WithMessage("{PropertyName} must not be null.")
                    .NotEmpty()
                    .WithMessage("{PropertyName} must not be empty.");

            }
            foreach (var lookupFeild in lookupFeilds)
            {
                var rule = RuleFor(GetNestedProperty<CreateMarriageEventCommand>(lookupFeild))
                    .Must(BeFoundInLookupTable)
                    .WithMessage("{PropertyName} with the provided id is not found");


            }
            var addressFeilds = new List<string>{
                "BrideInfo.BirthAddressId","BrideInfo.ResidentAddressId","Event.EventAddressId",
                "Event.EventOwener.BirthAddressId","Event.EventOwener.ResidentAddressId"
            };
            foreach (var addressFeild in addressFeilds)
            {
                var rule = RuleFor(GetNestedProperty<CreateMarriageEventCommand>(addressFeild))
                    .MustAsync(BeFoundInAddressTable)
                    .WithMessage("{PropertyName} with the provided id is not found");


            }
            RuleFor(e => e.Event.CivilRegOfficerId)
                .NotNull().WithMessage("civilRegOfficerId cannot be null")
                .NotEmpty().WithMessage("civilRegOfficerId cannot be empty")
                .Must(BeFoundInPersonalInfoTable).WithMessage("civilRegistrar officer with the provided id is not found");

            RuleFor(e => e.Event.CertificateId)
                .MustAsync(ValidateCertifcateId)
                .WithMessage("The last 4 digit of  {PropertyName} must be int., and must be unique.");

            RuleFor(e => e.Witnesses)
            .NotNull()
            .Must(w => w.Count >= 4).WithMessage("there should be atleast 4 witnesses to register a divorce");

            RuleFor(e => e.Witnesses.Select(w => w.WitnessForLookupId)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.FirstName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.MiddleName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.LastName)).NotEmpty().NotNull();
            
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.SexLookupId))
            .ForEach(lookupId => lookupId
                    .NotEmpty()
                    .NotNull()
                    .Must(BeFoundInLookupTable).WithMessage("witness sexLookup with the provided id is not found"));
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.Id))
                    .Must(NotHaveDuplicateWitness)
                    .WithMessage("duplicate witness personal info data: one person can only be registered as a witness once for a single marriage event");
            //only resident address is required
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.ResidentAddressId)).NotEmpty().NotNull();

            RuleFor(e => e.BrideInfo.BirthDateEt)
            .Must(BeAbove18YearsOld).WithMessage("the bride cannot be below 18 years old");
            RuleFor(e => e.Event.EventOwener.BirthDateEt)
            .Must(BeAbove18YearsOld).WithMessage("the Groom cannot be below 18 years old");

            When(e => isDivorcee(e.BrideInfo.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .NotNull()
                .NotEmpty()
                .MustAsync(async (model, supportingDocs, CancellationToken) => await haveDevorceCertificateAttachementAsync(supportingDocs, model.BrideInfo.Id, "wife")).WithMessage("divorce paper document should be attached if bride is a divorcee");
            });
            When(e => isDivorcee(e.Event.EventOwener.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .NotNull()
                .NotEmpty()
                .MustAsync(async (model, supportingDocs, CancellationToken) => await haveDevorceCertificateAttachementAsync(supportingDocs, model.Event.EventOwener.Id, "husband")).WithMessage("divorce paper document should be attached if eventOwner(Groom) is a divorcee");
            });
            When(e => isWidowed(e.Event.EventOwener.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .NotNull()
                .NotEmpty()
                .Must(haveDeathCertificateAttachement).WithMessage("Death Certificate document should be attached if eventOwner(Groom) is a Widowed");
            });
            When(e => isDivorcee(e.BrideInfo.MarriageStatusLookupId), () =>
           {
               RuleFor(e => e.Event.EventSupportingDocuments)
               .NotNull()
               .NotEmpty()
               .Must(haveDeathCertificateAttachement).WithMessage("death certificate paper document should be attached if bride is a divorcee");
           });
            When(e => !isCivilMarriage(e.MarriageTypeId), () =>
            {
                RuleFor(e => e.ApplicationId)
                .Must(id => id == null).WithMessage("MarriageApplicationId must be null if marriage type is not civil marriage");
            });
            When(e => isCivilMarriage(e.MarriageTypeId), () =>
            {
                RuleFor(e => e.ApplicationId)
                .NotNull().WithMessage("marriage application id is required for 'civil' marriage type")
                .NotEmpty().WithMessage("marriage application id cannot be empty for 'civil' marriage type")
                .Must(BeFoundInMarriageApplicationTable).WithMessage("marriage application with the provided id not found")
                .Must(BeUniqueApplicationId).WithMessage($"Duplicate MarriageApplicationID :  only one marriage event can be registered with one marriage application");

                RuleFor(e => e.Event.EventRegDateEt)
                .MustAsync(async (model, eventRegDateEt, CancellationToken) => await Be15DaysAfterMarriageApplicationDateAsync(eventRegDateEt, model))
                .WithMessage("there should be atleast 15 day gap between marriage application date and marriage registered date");
            });
            When(e => !isReligionMarriage(e.MarriageTypeId), () =>
          {
              RuleFor(e => e.BrideInfo.Id)
              .Must(BeUnmarried).WithMessage("Bride cannot be mairried : \n polygammy is prohibited for civil and cultural marriage");
              RuleFor(e => e.Event.EventOwener.Id)
              .Must(BeUnmarried).WithMessage("Groom cannot be mairried : \n polygammy is prohibited for civil and cultural marriage");
          });
            When(e => !e.Event.IsExampted, () =>
            {
                RuleFor(e => e.Event.PaymentExamption)
                .Must(pe => pe == null).WithMessage("payment examption must be null if isExampted = false");
            });
            When(e => e.Event.IsExampted, () =>
            {
                RuleFor(e => e.Event.PaymentExamption)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty().WithMessage("payment Examption cannot be empty if isExapmted = true")
                    .NotNull().WithMessage("payment Examption cannot be null if isExapmted = true");
                RuleFor(e => e.Event.PaymentExamption)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull().WithMessage("paymentExamptionRequestId cannot be null")
                    .NotEmpty().WithMessage("paymentExamptionRequestId cannot be empty")
                    .Must(BeFoundInExamptionRequestTable).WithMessage("paymentExamptionRequest with the provided id is not found");
            });

        }

        private bool notEmptyGuid(object arg)
        {
            try
            {

                Guid.TryParse(arg.ToString(), out Guid converted);
                return converted != Guid.Empty;
            }
            catch (System.Exception)
            {

                return false;
            }

        }
        private async Task<bool> ValidateCertifcateId(string CertId, CancellationToken token)
        {
            var valid = int.TryParse(CertId.Substring(CertId.Length - 4), out _);
            if (valid)
            {
                var certfcate = _eventRepo.GetAll().Where(x => x.CertificateId == CertId).FirstOrDefault();
                if (certfcate == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        private bool BeUnmarried(Guid? personalInfoId)
        {
            var isMarried = _marriageEventRepo.GetAll()
                            .Where(m => (m.BrideInfoId == personalInfoId || m.Event.EventOwenerId == personalInfoId) && !m.IsDivorced)
                            .Any();
            return personalInfoId == null || !isMarried;

        }

        private bool NotHaveDuplicateWitness(IEnumerable<Guid?> personalIfoIds)
        {
            var withoutNulls = personalIfoIds.Where(id => id != null && id != Guid.Empty);
            return withoutNulls.Count() == withoutNulls.Distinct().Count();
        }
        private bool BeFoundInExamptionRequestTable(AddPaymentExamptionRequest? paymentExamption)
        {
            return paymentExamption == null || _paymentExamptionRequestRepo.exists(paymentExamption!.ExamptionRequestId);
        }

        private bool BeUniqueApplicationId(Guid? marriageApplicationId)
        {
            return marriageApplicationId == null || !(_marriageEventRepo.GetAllQueryableAsync().Where(m => m.ApplicationId == marriageApplicationId).Any());
        }

        private async Task<bool> BeFoundInAddressTable(object addressId, CancellationToken token)
        {
            return addressId != null && (await _addressRepo.GetAsync(addressId)) != null;
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


        private async Task<bool> Be15DaysAfterMarriageApplicationDateAsync(string marriageRegDateEt, CreateMarriageEventCommand marriageEvent)
        {
            var application = await _marriageApplicationRepo.GetAsync(marriageEvent.ApplicationId!);
            var converted = new CustomDateConverter(marriageRegDateEt).gorgorianDate;


            return application == null
            || (converted - application?.ApplicationDate)?.Days >= 15;
        }

        private bool BeFoundInMarriageApplicationTable(Guid? applicationId)
        {
            return applicationId == null || _marriageApplicationRepo.exists((Guid)applicationId);
        }

        private bool isCivilMarriage(Guid marriageTypeId)
        {
            var marriageType = _lookupRepo.GetLookupById(marriageTypeId);
            return marriageType == null ||
             marriageType.Value.Value<string>("or")?.ToLower() == EnumDictionary.marriageTypeDict[MarriageType.Civil].or!.ToLower()
             || marriageType.Value.Value<string>("am")?.ToLower() == EnumDictionary.marriageTypeDict[MarriageType.Civil].am!.ToLower();
            ;
        }

        private async Task<bool> haveDevorceCertificateAttachementAsync(ICollection<AddSupportingDocumentRequest>? supportingDocs, Guid? perosnalInfoId, string type)
        {
            bool hasRegisteredDivorceCertificate = false;
            if (type == "wife")
            {

                hasRegisteredDivorceCertificate = perosnalInfoId != null
                         && await _divorceEventRepo.GetAllQueryableAsync().Where(e => e.DivorcedWifeId == perosnalInfoId).AnyAsync();
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
            return marriageStatus.Value.Value<string>("en")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedMan].en!.ToLower()
                    || marriageStatus.Value.Value<string>("en")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus. divorcedWoman].en!.ToLower()
                    || marriageStatus.Value.Value<string>("am")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedMan].am!.ToLower()
                    || marriageStatus.Value.Value<string>("am")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus. divorcedWoman].am!.ToLower()
                    || marriageStatus.Value.Value<string>("or")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedMan].or!.ToLower()
                    || marriageStatus.Value.Value<string>("or")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus. divorcedWoman].or!.ToLower();
        }
        private bool isWidowed(Guid marriageStatusLookupId)
        {
            var marriageStatus = _lookupRepo.GetLookupById(marriageStatusLookupId);
            if (marriageStatus == null)
            {
                return false;
            }
            return marriageStatus.Value.Value<string>("en")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.widowedMan].en!.ToLower()
                    || marriageStatus.Value.Value<string>("en")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.widowedWoman].en!.ToLower()
                    || marriageStatus.Value.Value<string>("am")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.widowedMan].am!.ToLower()
                    || marriageStatus.Value.Value<string>("am")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.widowedWoman].am!.ToLower()
                    || marriageStatus.Value.Value<string>("or")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.widowedMan].or!.ToLower()
                    || marriageStatus.Value.Value<string>("or")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.widowedWoman].or!.ToLower();
        }

        private bool BeAbove18YearsOld(string birthDate)
        {
            DateTime converted = new CustomDateConverter(birthDate).gorgorianDate;
            return DateTime.Now.Year - converted.Year >= 18;
        }

        public bool isReligionMarriage(Guid marriageTypeId)
        {
            var marriageType = _lookupRepo.GetLookupById(marriageTypeId);
            if (marriageType == null)
            {
                return false;
            }
            return marriageType.ValueStr.ToLower()
                    .Contains(EnumDictionary.marriageTypeDict[MarriageType.Religion].ToString()!.ToLower()
                    );

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