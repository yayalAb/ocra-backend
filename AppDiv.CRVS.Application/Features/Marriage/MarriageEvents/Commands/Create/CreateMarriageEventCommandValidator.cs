using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.Marriage.MarriageEvents.Commands;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Persistence.Couch;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Validators;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Utility.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
        private readonly ISettingRepository _settingRepository;
        private readonly IMarriageApplicationCouchRepository _marraigeApplicationCouchRepo;
        private readonly IEventRepository _eventRepo;
        private readonly Guid? _divorceTypeLookupId;

        [Obsolete]
        public CreateMarriageEventCommandValidator(ILookupRepository lookupRepo,
                                                   IMarriageApplicationRepository marriageApplicationRepo,
                                                   IPersonalInfoRepository personalInfoRepo,
                                                   IDivorceEventRepository divorceEventRepo,
                                                   IMarriageEventRepository marriageEventRepo,
                                                   IPaymentExamptionRequestRepository paymentExamptionRequestRepo,
                                                   IAddressLookupRepository addressRepo,
                                                   ISettingRepository settingRepository,
                                                   IMarriageApplicationCouchRepository marraigeApplicationCouchRepo,
                                                   IEventRepository eventRepo)
        {
            _lookupRepo = lookupRepo;
            _marriageApplicationRepo = marriageApplicationRepo;
            _personalInfoRepo = personalInfoRepo;
            _divorceEventRepo = divorceEventRepo;
            _marriageEventRepo = marriageEventRepo;
            _paymentExamptionRequestRepo = paymentExamptionRequestRepo;
            _addressRepo = addressRepo;
            _settingRepository = settingRepository;
            _marraigeApplicationCouchRepo = marraigeApplicationCouchRepo;
            _eventRepo = eventRepo;
            _divorceTypeLookupId = _lookupRepo.GetAll()
                .Where(l => l.ValueStr.ToLower()
                .Contains(Enum.GetName<SupportingDcoumentType>(SupportingDcoumentType.DivorcePaper)!.ToLower()))
                .Select(l => l.Id).FirstOrDefault();

            // var nonRequiredfieldNames = new List<string> 
            // {
            //     "BrideInfo.NationalId",
            //     "BrideInfo.ReligionLookupId","BrideInfo.ResidentAddressId",
            //     "BrideInfo.EducationalStatusLookupId","BrideInfo.TypeOfWorkLookupId","BrideInfo.MarriageStatusLookupId",
            //     "BrideInfo.BirthAddressId","BrideInfo.NationLookupId","Event.CertificateId","Event.EventAddressId",
            //     "Event.EventOwener.NationalId","Event.EventOwener.ReligionLookupId",
            //     "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
            //     "Event.EventOwener.ResidentAddressId","Event.EventOwener.BirthAddressId","Event.EventOwener.NationLookupId",

            // };
            var fieldNames =
            new List<string>{
                "MarriageTypeId","BrideInfo",
                    "BrideInfo.FirstName","BrideInfo.MiddleName","BrideInfo.LastName",
                    "BrideInfo.BirthDateEt","BrideInfo.NationalityLookupId",
                    "Event.EventDateEt",
                    "Event.EventRegDateEt","Event.CivilRegOfficerId","Event.IsExampted",
                    "Event.EventOwener.FirstName","Event.EventOwener.MiddleName","Event.EventOwener.LastName","Event.EventOwener.BirthDateEt",
                    "Event.EventOwener.NationalityLookupId",
            };
            // var nonRequiredLookupFields = new List<string>
            // {

            // };
            var lookupFeilds = new List<string>{
               "MarriageTypeId",
                    "BrideInfo.NationalityLookupId",
                    "Event.EventOwener.NationalityLookupId",
                    // "BrideInfo.ReligionLookupId",
                    // "BrideInfo.EducationalStatusLookupId","BrideInfo.TypeOfWorkLookupId","BrideInfo.MarriageStatusLookupId",
                    // "BrideInfo.NationLookupId",
                    // "Event.EventOwener.ReligionLookupId",
                    // "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
                    // "Event.EventOwener.NationLookupId"
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
                    .MustAsync(async (l, c) => await BeFoundInLookupTable(l))
                    .WithMessage("{PropertyName} with the provided id is not found");
                // .When(p => GetNestedProperty<CreateMarriageEventCommand>(lookupFeild) != null);


            }
            var addressFeilds = new List<string>
            {
                // "BrideInfo.BirthAddressId","BrideInfo.ResidentAddressId","Event.EventAddressId",
                // "Event.EventOwener.BirthAddressId","Event.EventOwener.ResidentAddressId"
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
                .WithMessage("The last 4 digit of  {PropertyName} must be int., and must be unique.")
                .When(e => e.Event.CertificateId != null);

            RuleFor(e => e.Witnesses.Count)
            .NotNull()
            .Must(meetMinimumWitnessCount)
            .WithMessage("number of witnesses should be equal or greater than the limit set in marriage setting or 4 if not set");

            RuleFor(e => e.Witnesses.Select(w => w.WitnessForLookupId)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.FirstName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.MiddleName)).NotEmpty().NotNull();
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.LastName)).NotEmpty().NotNull();

            // RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.SexLookupId))
            // .ForEach(lookupId => lookupId
            //         .NotEmpty()
            //         .NotNull()
            //         .MustAsync(async (l, c) => await BeFoundInLookupTable(l)).WithMessage("witness sexLookup with the provided id is not found"));
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.Id))
                    .Must(NotHaveDuplicateWitness)
                    .WithMessage("duplicate witness personal info data: one person can only be registered as a witness once for a single marriage event");
            //only resident address is required
            // RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.ResidentAddressId)).NotEmpty().NotNull()
            // .When(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.ResidentAddressId) != null);

            RuleFor(e => e.BrideInfo.BirthDateEt)
            .Must((e, birthDate) => BeAboveTheAgeLimit(birthDate, e.Event.EventDateEt, true, e.Event.EventSupportingDocuments))
            .WithMessage("the bride cannot be below the age limit set in setting or must attach underage marriage approval supporting document");
            RuleFor(e => e.Event.EventOwener.BirthDateEt)
            .Must((e, birthDate) => BeAboveTheAgeLimit(birthDate, e.Event.EventDateEt, false, e.Event.EventSupportingDocuments))
            .WithMessage("the Groom cannot be below the age limit set in setting or must attach underage marriage approval supporting document");

            WhenAsync(async (e, CancellationToken) => await isDivorcee(e.BrideInfo.MarriageStatusLookupId), () =>
            {

                RuleFor(e => e.BrideInfo.Id)
                .Must((e, brideId) => meetMinimumDivorceMarriageGapLimit(brideId, e.Event.EventDateEt, e.Event.EventSupportingDocuments))
                .WithMessage("divorced bride must wait month limit set in setting to marry again or attach pregnancy free certificate");

                // RuleFor(e => e.Event.EventSupportingDocuments)
                // .NotNull()
                // .NotEmpty()
                // .MustAsync(async (model, supportingDocs, CancellationToken) => await haveDevorceCertificateAttachementAsync(supportingDocs, model.BrideInfo.Id, "wife")).WithMessage("divorce paper document should be attached if bride is a divorcee");
            });
            WhenAsync(async (e, c) => await isDivorcee(e.Event.EventOwener.MarriageStatusLookupId), () =>
            {
                // RuleFor(e => e.Event.EventSupportingDocuments)
                // .NotNull()
                // .NotEmpty()
                // .MustAsync(async (model, supportingDocs, CancellationToken) => await haveDevorceCertificateAttachementAsync(supportingDocs, model.Event.EventOwener.Id, "husband")).WithMessage("divorce paper document should be attached if eventOwner(Groom) is a divorcee");
            });
            WhenAsync(async (e, c) => await isWidowed(e.Event.EventOwener.MarriageStatusLookupId), () =>
            {
                // RuleFor(e => e.Event.EventSupportingDocuments)
                // .NotNull()
                // .NotEmpty()
                // .Must(haveDeathCertificateAttachement).WithMessage("Death Certificate document should be attached if eventOwner(Groom) is a Widowed");
            });
            WhenAsync(async (e, c) => await isWidowed(e.BrideInfo.MarriageStatusLookupId), () =>
           {


               //    RuleFor(e => e.Event.EventSupportingDocuments)
               //    .NotNull()
               //    .NotEmpty()
               //    .Must(haveDeathCertificateAttachement).WithMessage("death certificate paper document should be attached if bride is a widow");
           });
            WhenAsync(async (e, c) => !(await isCivilMarriage(e.MarriageTypeId)), () =>
            {
                RuleFor(e => e.ApplicationId)
                .Must(id => id == null).WithMessage("MarriageApplicationId must be null if marriage type is not civil marriage");
            });
            WhenAsync(async (e, c) => await isCivilMarriage(e.MarriageTypeId), () =>
            {
                RuleFor(e => e.ApplicationId)
                .NotNull().WithMessage("marriage application id is required for 'civil' marriage type")
                .NotEmpty().WithMessage("marriage application id cannot be empty for 'civil' marriage type")
                .Must((e, applicationId, CancellationToken) =>
                    BeFoundInMarriageApplicationTable(applicationId) || e.Application != null
                    // || ((e.Event.EventRegisteredAddressId != null)
                    //     && await (BeFoundInMarriageApplicationCouch(applicationId, (Guid)e.Event.EventRegisteredAddressId, CancellationToken))
                    // )
                    )
                    .WithMessage("marriage application with the provided id not found ,")
                .Must(BeUniqueApplicationId).WithMessage($"Duplicate MarriageApplicationID :  only one marriage event can be registered with one marriage application");

                RuleFor(e => e.Event.EventRegDateEt)
                .MustAsync(async (model, eventRegDateEt, CancellationToken) => await Be15DaysAfterMarriageApplicationDateAsync(eventRegDateEt, model))
                .WithMessage("there should be atleast 15 day gap between marriage application date and marriage registered date");
            });
            WhenAsync(async (e, c) => !await isReligionMarriage(e.MarriageTypeId), () =>
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
                // RuleFor(e => e.Event.PaymentExamption)
                //     .Cascade(CascadeMode.StopOnFirstFailure)
                //     .NotNull().WithMessage("paymentExamptionReasonId cannot be null")
                //     .NotEmpty().WithMessage("paymentExamptionReasonId cannot be empty")
                //     .Must(BeFoundInLookupTable).WithMessage("paymentExamptionRequest with the provided id is not found");
            });
            When(p => p.Event.EventSupportingDocuments != null, () =>
            {
                RuleFor(p => p.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator("Event.EventSupportingDocuments")!);
            });
            RuleFor(p => p.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(eventRepo)!)
                    .When(p => (p.Event.IsExampted));
            When(p => p.Event.PaymentExamption?.SupportingDocuments != null, () =>
            {
                RuleFor(p => p.Event.PaymentExamption.SupportingDocuments)
                .SetValidator(new SupportingDocumentsValidator("Event.PaymentExamption.SupportingDocuments")!);
            });

        }

        private bool BeAboveTheAgeLimit(string birthDate, string eventDateEt, bool isBride, ICollection<AddSupportingDocumentRequest>? supportingDocs)
        {
            var beAboveAgeLimit = MarriageValidatorFunctions.IsAboveTheAgeLimit(birthDate, eventDateEt, isBride, _settingRepository);
            var hasUnderAgeMarriageApprovalDoc = supportingDocs != null && MarriageValidatorFunctions.hasSupportingDoc(supportingDocs, _lookupRepo, "underage marriage approval");
            return beAboveAgeLimit || hasUnderAgeMarriageApprovalDoc;
        }

        private bool meetMinimumDivorceMarriageGapLimit(Guid? brideId, string eventDateEt, ICollection<AddSupportingDocumentRequest> supportingDocs)
        {
            if (brideId == null)
            {
                return true;
            }
            var bridehaseDivorce = MarriageValidatorFunctions.brideHasDivorceInLessThanDateLimitInSetting((Guid)brideId, eventDateEt, _settingRepository, _personalInfoRepo);
            var hasPregnancyFreeSupportingDoc = MarriageValidatorFunctions.hasSupportingDoc(supportingDocs, _lookupRepo, "pregnancy free certificate");
            return !bridehaseDivorce || hasPregnancyFreeSupportingDoc;
        }

        private bool meetMinimumWitnessCount(int numberOfWitnesses)
        {
            var marriageSetting = _settingRepository.GetAll()
                    .Where(s => s.Key == "marriageSetting")
                    .FirstOrDefault();
            if (marriageSetting == null)
            {
                throw new NotFoundException("marriage setting not found");
            }
            var minWitnessCount = marriageSetting.Value.Value<string>("how_many_witness");

            if (int.TryParse(minWitnessCount, out int result))
            {
                return numberOfWitnesses >= result;
            }
            else
            {
                return numberOfWitnesses >= 4;
            }
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
                var certfcate = await _eventRepo.GetAll().Where(x => x.CertificateId == CertId && x.EventType == "Marriage").FirstOrDefaultAsync();
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

        private bool BeUniqueApplicationId(Guid? marriageApplicationId)
        {
            return marriageApplicationId == null || !(_marriageEventRepo.GetAllQueryableAsync().Where(m => m.ApplicationId == marriageApplicationId).Any());
        }

        private async Task<bool> BeFoundInAddressTable(object addressId, CancellationToken token)
        {
            if (addressId == null)
                return true;
            return addressId != null && (await _addressRepo.GetAsync(addressId)) != null;
        }

        private bool BeFoundInPersonalInfoTable(Guid guid)
        {
            return _personalInfoRepo.GetById(guid) != null;
        }

        private async Task<bool> BeFoundInLookupTable(object? lookupId)
        {
            if (lookupId == null)
                return true;
            var l = lookupId;
            // return false;

            return lookupId != null && await _lookupRepo.GetLookupById((Guid)lookupId) != null;
        }
        private async Task<bool> BeFoundInLookupTable(Guid? lookupId)
        {
            if (lookupId == null)
                return true;
            return lookupId != null && await _lookupRepo.GetLookupById((Guid)lookupId) != null;
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
        private async Task<bool> BeFoundInMarriageApplicationCouch(Guid? applicationId, Guid registrationAddressId, CancellationToken cancellationToken)
        {
            if (applicationId == null)
            {
                return true;
            }
            var res = await _marraigeApplicationCouchRepo.Exists((Guid)applicationId, registrationAddressId);
            if (!res.Success)
            {
                return false;
            }
            var syncRes = await _marraigeApplicationCouchRepo.SyncMarraigeApplication(res.marriageApplication!, registrationAddressId, cancellationToken);
            return syncRes.Success;

        }

        private async Task<bool> isCivilMarriage(Guid marriageTypeId)
        {
            var marriageType = await _lookupRepo.GetLookupById(marriageTypeId);
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


            return hasRegisteredDivorceCertificate || supportingDocs == null || _divorceTypeLookupId == null || supportingDocs.ToList()
             .Where(doc => doc.Type == _divorceTypeLookupId)
             .Any();
        }
        private bool haveDeathCertificateAttachement(ICollection<AddSupportingDocumentRequest>? supportingDocs)
        {
            return supportingDocs == null || _divorceTypeLookupId == null || supportingDocs.ToList()
             .Where(doc => doc.Type == _divorceTypeLookupId)
             .Any();
        }


        private async Task<bool> isDivorcee(Guid? marriageStatusLookupId)
        {
            if (marriageStatusLookupId == null)
                return false;
            var marriageStatus = await _lookupRepo.GetLookupById((Guid)marriageStatusLookupId);
            if (marriageStatus == null)
            {
                return false;
            }
            return marriageStatus?.Value?.Value<string>("en")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedMan].en!.ToLower()
                    || marriageStatus?.Value?.Value<string>("en")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedWoman].en!.ToLower()
                    || marriageStatus?.Value?.Value<string>("am")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedMan].am!.ToLower()
                    || marriageStatus?.Value?.Value<string>("am")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedWoman].am!.ToLower()
                    || marriageStatus?.Value?.Value<string>("or")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedMan].or!.ToLower()
                    || marriageStatus?.Value?.Value<string>("or")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedWoman].or!.ToLower();
        }
        private async Task<bool> isWidowed(Guid? marriageStatusLookupId)
        {
            if (marriageStatusLookupId == null)
                return false;
            var marriageStatus = await _lookupRepo.GetLookupById((Guid)marriageStatusLookupId);
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



        public async Task<bool> isReligionMarriage(Guid marriageTypeId)
        {
            var marriageType = await _lookupRepo.GetLookupById(marriageTypeId);
            return marriageType == null ||
                  marriageType.Value.Value<string>("or")?.ToLower() == EnumDictionary.marriageTypeDict[MarriageType.Religion].or!.ToLower()
                  || marriageType.Value.Value<string>("am")?.ToLower() == EnumDictionary.marriageTypeDict[MarriageType.Religion].am!.ToLower();
            ;

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