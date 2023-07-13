using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using System.Linq.Expressions;


namespace AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update
{
    public class UpdateDivorceEventCommandValidator : AbstractValidator<UpdateDivorceEventCommand>
    {
        private readonly IPersonalInfoRepository _personalInfoRepo;
        private readonly ILookupRepository _lookupRepo;
        private readonly IAddressLookupRepository _addressRepo;
        private readonly IEventRepository _eventRepo;
        private readonly ICourtRepository _courtRepo;
        public UpdateDivorceEventCommandValidator(IPersonalInfoRepository personalInfoRepo,
                                                  ILookupRepository lookupRepo,
                                                  IAddressLookupRepository addressRepo,
                                                  IEventRepository eventRepo,
                                                  ICourtRepository courtRepo)
        {
            _personalInfoRepo = personalInfoRepo;
            _lookupRepo = lookupRepo;
            _addressRepo = addressRepo;
            _eventRepo = eventRepo;
            _courtRepo = courtRepo;
            var fieldNames = new List<string>{"DivorcedWife","DivorcedWife.Id","DateOfMarriageEt", 
            // "DivorceReason", "NumberOfChildren",
            "Event","CourtCase",
            "DivorcedWife.FirstName","DivorcedWife.MiddleName","DivorcedWife.LastName","DivorcedWife.NationalityLookupId",
            // "DivorcedWife.NationalId",
            // "DivorcedWife.BirthAddressId","DivorcedWife.ReligionLookupId","DivorcedWife.EducationalStatusLookupId",
            // "DivorcedWife.TypeOfWorkLookupId","DivorcedWife.MarriageStatusLookupId","DivorcedWife.ResidentAddressId","DivorcedWife.NationLookupId",
            "CourtCase","CourtCase.Id",
            // "CourtCase.CourtCaseNumber","Event.CertificateId","Event.EventAddressId",
            "CourtCase.ConfirmedDateEt",
            "Event.Id", "Event.EventDateEt",
            "Event.EventRegDateEt","Event.CivilRegOfficerId","Event.IsExampted",
            "Event.EventOwener.FirstName","Event.EventOwener.MiddleName","Event.EventOwener.LastName","Event.EventOwener.NationalityLookupId",
            // "Event.EventOwener.NationalId",
            // "Event.EventOwener.ReligionLookupId",
            // "Event.EventOwener.EducationalStatusLookupId","Event.EventOwener.TypeOfWorkLookupId","Event.EventOwener.MarriageStatusLookupId",
            // "Event.EventOwener.ResidentAddressId","Event.EventOwener.BirthAddressId","Event.EventOwener.NationLookupId",
            };
            foreach (var fieldName in fieldNames)
            {
                var rule = RuleFor(GetNestedProperty<UpdateDivorceEventCommand>(fieldName))
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
                var rule = RuleFor(GetNestedProperty<UpdateDivorceEventCommand>(lookupFeild))
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .MustAsync(async (lookupId, _) => await BeFoundInLookupTable(lookupId))
                    .WithMessage("{PropertyName} with the provided id is not found");

                // add more validation rules for the field here, if needed
            }
            var addressFeilds = new List<string>{
                "DivorcedWife.BirthAddressId","DivorcedWife.ResidentAddressId","Event.EventAddressId",
                "Event.EventOwener.BirthAddressId","Event.EventOwener.ResidentAddressId"
            };
            foreach (var addressFeild in addressFeilds)
            {
                var rule = RuleFor(GetNestedProperty<UpdateDivorceEventCommand>(addressFeild))
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

            RuleFor(e => e.DivorcedWife.Id)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotNull()
            .NotEmpty()
            .Must(BeFoundInPersonalInfoTable).WithMessage("devorced wife with the provided id is not found");
            RuleFor(e => e.Event.EventOwener.Id)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotNull()
            .NotEmpty()
            .Must(BeFoundInPersonalInfoTable).WithMessage("Event owner with the provided id is not found");

            RuleFor(e => e.CourtCase.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .Must(BeFoundInCourtCaseTable).WithMessage("Courtcase with the provided id is not found");
            RuleFor(e => e.Event.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .Must(BeFoundInEventTable).WithMessage("event with the provided id is not found");

        }

        private bool BeFoundInEventTable(Guid? id)
        {
            return id == null || _eventRepo.GetAllQueryableAsync().Where(e => e.Id == id).Any();
        }

        private bool BeFoundInCourtCaseTable(Guid? id)
        {
            return id == null || _courtRepo.CourtCaseExists((Guid)id);
        }

        private async Task<bool> BeFoundInCourtTable(Guid? courtId, CancellationToken token)
        {
            return courtId != null && (await _courtRepo.GetAsync(courtId)) != null;
        }

        private async Task<bool> BeFoundInAddressTable(object addressId, CancellationToken token)
        {
            return addressId != null && (await _addressRepo.GetAsync(addressId)) != null;

        }

        private bool BeFoundInPersonalInfoTable(Guid? guid)
        {
            return _personalInfoRepo.GetById((Guid)guid) != null;
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