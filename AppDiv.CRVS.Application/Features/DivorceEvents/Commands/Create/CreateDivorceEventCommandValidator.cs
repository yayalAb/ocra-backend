
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Validators;
using FluentValidation;
using System.Linq.Expressions;


namespace AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create
{
    public class CreateDivorceEventCommandValidator : AbstractValidator<CreateDivorceEventCommand>
    {
        private readonly IPersonalInfoRepository _personalInfoRepo;
        private readonly ILookupRepository _lookupRepo;
        private readonly IAddressLookupRepository _addressRepo;
        private readonly ICourtRepository _courtRepo;
        private readonly IEventRepository _eventRepo;

        public CreateDivorceEventCommandValidator(IPersonalInfoRepository personalInfoRepo,
                                                  ILookupRepository lookupRepo,
                                                  IAddressLookupRepository addressRepo,
                                                  ICourtRepository courtRepo,
                                                  IEventRepository eventRepo)
        {
            _personalInfoRepo = personalInfoRepo;
            _lookupRepo = lookupRepo;
            _addressRepo = addressRepo;
            _courtRepo = courtRepo;
            _eventRepo = eventRepo;
            var fieldNames = new List<string>{"DivorcedWife","DateOfMarriageEt", 
            // "DivorceReason", "NumberOfChildren",
            "CourtCase","Event",
            "DivorcedWife.FirstName","DivorcedWife.MiddleName","DivorcedWife.LastName","DivorcedWife.NationalityLookupId",
            // "DivorcedWife.NationalId","DivorcedWife.BirthAddressId","DivorcedWife.ReligionLookupId","DivorcedWife.EducationalStatusLookupId",
            // "DivorcedWife.TypeOfWorkLookupId","DivorcedWife.MarriageStatusLookupId","DivorcedWife.ResidentAddressId","DivorcedWife.NationLookupId",
            // "CourtCase.CourtCaseNumber","Event.CertificateId","Event.EventAddressId",
            "CourtCase.ConfirmedDateEt",
            //  "Event.EventDateEt",
            "Event.EventRegDateEt","Event.CivilRegOfficerId","Event.IsExampted",
            "Event.EventOwener.FirstName","Event.EventOwener.MiddleName","Event.EventOwener.LastName","Event.EventOwener.NationalityLookupId",
            // "Event.EventOwener.NationalId",
            // "Event.EventOwener.ReligionLookupId",
            // "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
            // "Event.EventOwener.ResidentAddressId","Event.EventOwener.BirthAddressId","Event.EventOwener.NationLookupId",
            };
            foreach (var fieldName in fieldNames)
            {
                var rule = RuleFor(GetNestedProperty<CreateDivorceEventCommand>(fieldName))
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .WithMessage("{PropertyName} must not be null.")
                    .NotEmpty()
                    .WithMessage("{PropertyName} must not be empty.");

                // add more validation rules for the field here, if needed
            }
            var lookupFeilds = new List<string>{
                "DivorcedWife.NationalityLookupId",
                // "DivorcedWife.ReligionLookupId",
                // "DivorcedWife.EducationalStatusLookupId","DivorcedWife.TypeOfWorkLookupId","DivorcedWife.MarriageStatusLookupId",
                // "DivorcedWife.NationLookupId",
                "Event.EventOwener.NationalityLookupId",
                // "Event.EventOwener.ReligionLookupId",
                // "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
                // "Event.EventOwener.NationLookupId"
            };
            foreach (var lookupFeild in lookupFeilds)
            {
                var rule = RuleFor(GetNestedProperty<CreateDivorceEventCommand>(lookupFeild))
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .MustAsync(async (lookupId, _) => await BeFoundInLookupTable(lookupId))
                    .WithMessage("{PropertyName} with the provided id is not found");

                // add more validation rules for the field here, if needed
            }
            var addressFeilds = new List<string>
            {
                // "DivorcedWife.BirthAddressId","DivorcedWife.ResidentAddressId","Event.EventAddressId",
                // "Event.EventOwener.BirthAddressId","Event.EventOwener.ResidentAddressId"
            };
            foreach (var addressFeild in addressFeilds)
            {
                var rule = RuleFor(GetNestedProperty<CreateDivorceEventCommand>(addressFeild))
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .MustAsync(BeFoundInAddressTable)
                    .WithMessage("{PropertyName} with the provided id is not found");

                // add more validation rules for the field here, if needed
            }
            When(e => e.Event.IsExampted, () =>
         {
             RuleFor(e => e.Event.PaymentExamption)
                 .Cascade(CascadeMode.StopOnFirstFailure)
                 .NotEmpty().WithMessage("payment Examption cannot be empty if isExapmted = true")
                 .NotNull().WithMessage("payment Examption cannot be null if isExapmted = true");

         });
            RuleFor(e => e.Event.CivilRegOfficerId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("civilRegOfficerId cannot be null")
                .NotEmpty().WithMessage("civilRegOfficerId cannot be empty")
                .Must(BeFoundInPersonalInfoTable).WithMessage("civilRegistrar officer with the provided id is not found");
            When(e => e.CourtCase.Court.Id == null, () =>
           {
               RuleFor(e => e.CourtCase.Court)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .NotEmpty().WithMessage("court cannot be empty if courtId is null")
                   .NotNull().WithMessage("court cannot be null if courtId is null");

           });
            When(e => e.CourtCase.Court == null, () =>
            {
                RuleFor(e => e.CourtCase.Court.Id)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty().WithMessage("court cannot be empty if courtId is null")
                    .NotNull().WithMessage("court cannot be null if courtId is null")
                    .MustAsync(BeFoundInCourtTable).WithMessage("court with the specified id is not found");
            });
            RuleFor(e => e.Event.CertificateId)
                .MustAsync(ValidateCertifcateId)
                .WithMessage("The last 4 digit of  {PropertyName} must be int., and must be unique.")
                .When(e => e.Event.CertificateId != null);
            RuleFor(p => p.Event.EventSupportingDocuments).SetValidator(new SupportingDocumentsValidator("Event.EventSupportingDocuments")!)
                   .When(p => (p.Event.EventSupportingDocuments != null));
            RuleFor(p => p.Event.PaymentExamption).SetValidator(new PaymentExamptionValidator(eventRepo)!)
                    .When(p => (p.Event.IsExampted));
            When(p => p.Event.PaymentExamption?.SupportingDocuments != null, () =>
            {
                RuleFor(p => p.Event.PaymentExamption.SupportingDocuments)
                .SetValidator(new SupportingDocumentsValidator("Event.PaymentExamption.SupportingDocuments")!);
            });
        }

        private async Task<bool> ValidateCertifcateId(string CertId, CancellationToken token)
        {
            var valid = int.TryParse(CertId.Substring(CertId.Length - 4), out _);
            if (valid)
            {
                var certfcate = _eventRepo.GetAll().Where(x => x.CertificateId == CertId && x.EventType == "Divorce").FirstOrDefault();
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
        private async Task<bool> BeFoundInCourtTable(Guid? courtId, CancellationToken token)
        {
            return courtId != null && (await _courtRepo.GetAsync(courtId)) != null;
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
            return lookupId != null && await _lookupRepo.GetLookupById((Guid)lookupId) != null;
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