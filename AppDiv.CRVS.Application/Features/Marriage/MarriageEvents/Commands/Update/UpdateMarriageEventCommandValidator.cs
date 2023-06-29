
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Utility.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update
{
    public class UpdateMarriageEventCommandValidator : AbstractValidator<UpdateMarriageEventCommand>
    {
        private readonly ILookupRepository _lookupRepo;
        private readonly IMarriageApplicationRepository _marriageApplicationRepo;
        private readonly IPersonalInfoRepository _personalInfoRepo;
        private readonly IDivorceEventRepository _divorceEventRepo;
        private readonly IMarriageEventRepository _marriageEventRepo;
        private readonly IPaymentExamptionRequestRepository _paymentExamptionRequestRepo;
        private readonly IAddressLookupRepository _addressRepo;

        public UpdateMarriageEventCommandValidator(ILookupRepository lookupRepo, IMarriageApplicationRepository marriageApplicationRepo, IPersonalInfoRepository personalInfoRepo, IDivorceEventRepository divorceEventRepo, IMarriageEventRepository marriageEventRepo, IPaymentExamptionRequestRepository paymentExamptionRequestRepo, IAddressLookupRepository addressRepo)
        {
            _lookupRepo = lookupRepo;
            _marriageApplicationRepo = marriageApplicationRepo;
            _personalInfoRepo = personalInfoRepo;
            _divorceEventRepo = divorceEventRepo;
            _marriageEventRepo = marriageEventRepo;
            _paymentExamptionRequestRepo = paymentExamptionRequestRepo;
            _addressRepo = addressRepo;

            // RuleFor(e => e.Event.EventSupportingDocuments.Select(sd => sd.Id))


            var fieldNames =
            new List<string>{

                "Id","MarriageTypeId","BrideInfo","BrideInfo.Id","Event.Id","Event.EventOwener.Id",
                    "BrideInfo.FirstName","BrideInfo.MiddleName","BrideInfo.LastName","BrideInfo.BirthDateEt",
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
            foreach (var lookupFeild in lookupFeilds)
            {
                var rule = RuleFor(GetNestedProperty<UpdateMarriageEventCommand>(lookupFeild))
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .MustAsync(async (lookupId , c) => await BeFoundInLookupTable(lookupId))
                    .WithMessage("{PropertyName} with the provided id is not found");


            }
            foreach (var fieldName in fieldNames)
            {
                var rule = RuleFor(GetNestedProperty<UpdateMarriageEventCommand>(fieldName))
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .WithMessage("{PropertyName} must not be null.")
                    .NotEmpty()
                    .WithMessage("{PropertyName} must not be empty.");


            }
            var addressFeilds = new List<string>{
                "BrideInfo.BirthAddressId","BrideInfo.ResidentAddressId","Event.EventAddressId",
                "Event.EventOwener.BirthAddressId","Event.EventOwener.ResidentAddressId"
            };
            foreach (var addressFeild in addressFeilds)
            {
                var rule = RuleFor(GetNestedProperty<UpdateMarriageEventCommand>(addressFeild))
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .MustAsync(BeFoundInAddressTable)
                    .WithMessage("{PropertyName} with the provided id is not found");


            }
            RuleFor(e => e.Event.CivilRegOfficerId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("civilRegOfficerId cannot be null")
                .NotEmpty().WithMessage("civilRegOfficerId cannot be empty")
                .Must(BeFoundInPersonalInfoTable).WithMessage("civilRegistrar officer with the provided id is not found");



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
                    .MustAsync(async (lookupId , _) => await BeFoundInLookupTable(lookupId)).WithMessage("witness sexLookup with the provided id is not found"));
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.Id))
                    .Must(NotHaveDuplicateWitness)
                    .WithMessage("duplicate witness personal info data: one person can only be registered as a witness once for a single marriage event");

            //only resident address is required
            RuleFor(e => e.Witnesses.Select(w => w.WitnessPersonalInfo.ResidentAddressId)).NotEmpty().NotNull();

            RuleFor(e => e.BrideInfo.BirthDateEt)
            .Must(BeAbove18YearsOld).WithMessage("the bride cannot be below 18 years old");
            RuleFor(e => e.Event.EventOwener.BirthDateEt)
            .Must(BeAbove18YearsOld).WithMessage("the Groom cannot be below 18 years old");

            WhenAsync(async (e,c) => await isDivorcee(e.BrideInfo.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .MustAsync(async (model, supportingDocs, CancellationToken) => await haveDevorceCertificateAttachementAsync(supportingDocs, model.BrideInfo.Id, "wife")).WithMessage("divorce paper document should be attached if bride is a divorcee");
            });
            WhenAsync(async (e,c) => await isDivorcee(e.Event.EventOwener.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .MustAsync(async (model, supportingDocs, CancellationToken) => await haveDevorceCertificateAttachementAsync(supportingDocs, model.Event.EventOwener.Id, "husband")).WithMessage("divorce paper document should be attached if eventOwner(Groom) is a divorcee");
            });
            WhenAsync(async (e,c) => await isWidowed(e.Event.EventOwener.MarriageStatusLookupId), () =>
            {
                RuleFor(e => e.Event.EventSupportingDocuments)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .Must(haveDeathCertificateAttachement).WithMessage("Death Certificate document should be attached if eventOwner(Groom) is a Widowed");
            });
            WhenAsync(async (e,c) => await isDivorcee(e.BrideInfo.MarriageStatusLookupId), () =>
           {
               RuleFor(e => e.Event.EventSupportingDocuments)
               .Cascade(CascadeMode.StopOnFirstFailure)
               .NotNull()
               .NotEmpty()
               .Must(haveDeathCertificateAttachement).WithMessage("death certificate paper document should be attached if bride is a divorcee");
           });
            WhenAsync(async (e,c) => await isCivilMarriage(e.MarriageTypeId), () =>
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
            WhenAsync(async (e,c) => !(await isReligionMarriage(e.MarriageTypeId)), () =>
            {
                RuleFor(e => e.BrideInfo.Id)
                .Must((e, brideId) => BeUnmarried(brideId, e.Id)).WithMessage("Bride cannot be mairried : \n polygammy is prohibited for civil and cultural marriage");
                RuleFor(e => e.Event.EventOwener.Id)
               .Must((e, groomId) => BeUnmarried(groomId, e.Id)).WithMessage("Groom cannot be mairried : \n polygammy is prohibited for civil and cultural marriage");
            });
            When(e => e.Event.IsExampted, () =>
            {
                RuleFor(e => e.Event.PaymentExamption)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty().WithMessage("payment Examption cannot be empty if isExapmted = true")
                    .NotNull().WithMessage("payment Examption cannot be null if isExapmted = true");
                RuleFor(e => e.Event.PaymentExamption.ExamptionReasonLookupId)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull().WithMessage("paymentExamptionReasonLookupId cannot be null")
                    .NotEmpty().WithMessage("paymentExamptionReasonLookupId cannot be empty")
                    .Must(BeFoundInExamptionRequestTable).WithMessage("paymentExamptionRequest with the provided id is not found");
            });

        }

        private bool BeUnmarried(Guid? personalInfoId, Guid marriageId)
        {
            return personalInfoId == null || !_marriageEventRepo.GetAll()
                            .Where(m =>
                            (
                             m.BrideInfoId == personalInfoId
                            || m.Event.EventOwenerId == personalInfoId) && (m.Id != marriageId) && (!m.IsDivorced))
                            .Any();


        }

        private bool NotHaveDuplicateWitness(IEnumerable<Guid?> personalIfoIds)
        {
            var withoutNulls = personalIfoIds.Where(id => id != null && id != Guid.Empty);
            return withoutNulls.Count() == withoutNulls.Distinct().Count();
        }
        private bool BeFoundInExamptionRequestTable(Guid guid)
        {
            return _paymentExamptionRequestRepo.exists(guid);
        }

        private bool BeUniqueApplicationId(Guid? marriageApplicationId)
        {
            return _marriageEventRepo.GetAllQueryableAsync().Where(m => m.ApplicationId == marriageApplicationId).Any();
        }

        private async Task<bool> BeFoundInAddressTable(object addressId, CancellationToken token)
        {
            return addressId != null && (await _addressRepo.GetAsync(addressId)) != null;
        }

        private bool BeFoundInPersonalInfoTable(Guid guid)
        {
            return _personalInfoRepo.GetById(guid) != null;
        }

        private async Task<bool> BeFoundInLookupTable(object lookupId)
        {
            var l = lookupId;
            // return false;

            return lookupId != null && await _lookupRepo.GetLookupById((Guid)lookupId) != null;
        }
        private async Task<bool> BeFoundInLookupTable(Guid lookupId)
        {
            return lookupId != null && await _lookupRepo.GetLookupById((Guid)lookupId) != null;
        }


        private async Task<bool> Be15DaysAfterMarriageApplicationDateAsync(string marriageRegDateEt, UpdateMarriageEventCommand marriageEvent)
        {
            var application = await _marriageApplicationRepo.GetAsync(marriageEvent.ApplicationId!);
            var converted = new CustomDateConverter(marriageRegDateEt).gorgorianDate;


            return (converted - application.ApplicationDate).Days >= 15;
        }


        private bool BeFoundInMarriageApplicationTable(Guid? applicationId)
        {
            return applicationId != null && _marriageApplicationRepo.exists((Guid)applicationId);
        }


        private async Task<bool> isCivilMarriage(Guid marriageTypeId)
        {
            var marriageType = await _lookupRepo.GetLookupById(marriageTypeId);
            return marriageType == null ||
             marriageType.Value.Value<string>("or")?.ToLower() == EnumDictionary.marriageTypeDict[MarriageType.Civil].or!.ToLower()
             || marriageType.Value.Value<string>("am")?.ToLower() == EnumDictionary.marriageTypeDict[MarriageType.Civil].am!.ToLower();
            ;
        }
        public async Task<bool> isReligionMarriage(Guid marriageTypeId)
        {
            var marriageType = await _lookupRepo.GetLookupById(marriageTypeId);
            return marriageType == null ||
                  marriageType.Value.Value<string>("or")?.ToLower() == EnumDictionary.marriageTypeDict[MarriageType.Religion].or!.ToLower()
                  || marriageType.Value.Value<string>("am")?.ToLower() == EnumDictionary.marriageTypeDict[MarriageType.Religion].am!.ToLower();
            ;

        }
        // public bool BeUnmarried(Guid marriageStatusId)
        // {
        //     var marriageStatus = _lookupRepo.GetLookupById(marriageStatusId);
        //     if (marriageStatus == null)
        //     {
        //         return false;
        //     }
        //     return !(marriageStatus.ValueStr.ToLower()
        //             .Contains(EnumDictionary.marriageStatusDict[MarriageStatus.married].ToString()!.ToLower()));
        // }
// x
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


        private async Task<bool> isDivorcee(Guid marriageStatusLookupId)
        {
            var marriageStatus = await _lookupRepo.GetLookupById(marriageStatusLookupId);
            if (marriageStatus == null)
            {
                return false;
            }
            return marriageStatus.Value.Value<string>("en")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedMan].en!.ToLower()
                    || marriageStatus.Value.Value<string>("en")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedWoman].en!.ToLower()
                    || marriageStatus.Value.Value<string>("am")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedMan].am!.ToLower()
                    || marriageStatus.Value.Value<string>("am")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedWoman].am!.ToLower()
                    || marriageStatus.Value.Value<string>("or")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedMan].or!.ToLower()
                    || marriageStatus.Value.Value<string>("or")?.ToLower() == EnumDictionary.marriageStatusDict[MarriageStatus.divorcedWoman].or!.ToLower();
        }
        private async Task<bool> isWidowed(Guid marriageStatusLookupId)
        {
            var marriageStatus = await  _lookupRepo.GetLookupById(marriageStatusLookupId);
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