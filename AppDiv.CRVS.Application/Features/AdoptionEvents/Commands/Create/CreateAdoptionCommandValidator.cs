using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create
{
    public class CreatAdoptionCommandValidator : AbstractValidator<CreateAdoptionCommand>
    {
        private readonly IAdoptionEventRepository _repo;
        private readonly IAddressLookupRepository _address;

        private readonly IPersonalInfoRepository _PersonalInfo;
        private readonly ILookupRepository _LookupsRepo;
        private readonly IPaymentExamptionRequestRepository _PaymentExaptionRepo;
        private readonly IEventRepository _EventRepository;
        private readonly CustomDateConverter _dateConverter;

        public CreatAdoptionCommandValidator(IAdoptionEventRepository repo, IAddressLookupRepository address,
        IPersonalInfoRepository PersonalInfo, ILookupRepository LookupsRepo,
        IPaymentExamptionRequestRepository PaymentExaptionRepo,
        IEventRepository EventRepository
        )
        {
            _dateConverter = new CustomDateConverter();
            _repo = repo;
            _address = address;
            _PersonalInfo = PersonalInfo;
            _LookupsRepo = LookupsRepo;
            _PaymentExaptionRepo = PaymentExaptionRepo;
            _EventRepository = EventRepository;
            // RuleFor(e => e.Adoption.Id)
            //    .MustAsync(CheckIdOnCeate)
            //    .WithMessage("A {PropertyName} must be null on create.");
            RuleFor(e => e.Adoption.AdoptiveFather)
               .Must((e, adoptiveFather) => adoptiveFather != null || e.Adoption.AdoptiveMother != null).WithMessage("both adoptive mother and father cannot be null when registering adoption");
            // When(e => e.Adoption.AdoptiveFather != null, () =>
            // {
            //     RuleFor(e => e.Adoption.Event.EventOwener.MiddleName)
            //         .Cascade(CascadeMode.StopOnFirstFailure)
            //         .NotNull()
            //         .NotEmpty()
            //         .Must((e, childMiddleName) => BeEqual(childMiddleName, e.Adoption.AdoptiveFather!.FirstName)).WithMessage("the Child's MiddleName must be same as the Adoptive Father's Firstname");
            //     RuleFor(e => e.Adoption.Event.EventOwener.LastName)
            //         .Cascade(CascadeMode.StopOnFirstFailure)
            //         .NotNull()
            //         .NotEmpty()
            //         .Must((e, childLastName) => BeEqual(childLastName, e.Adoption.AdoptiveFather!.LastName)).WithMessage("the Child's lastname must be same as the Adoptive Father's MiddleName");

            // });
            // When(e => e.Adoption.AdoptiveFather == null && e.Adoption.AdoptiveMother != null, () =>
            // {
            //     RuleFor(e => e.Adoption.Event.EventOwener.MiddleName)
            //         .Cascade(CascadeMode.StopOnFirstFailure)
            //         .NotNull()
            //         .NotEmpty()
            //         .Must((e, childMiddleName) => BeEqual(childMiddleName, e.Adoption.AdoptiveMother!.MiddleName)).WithMessage("the Child's MiddleName must be same as the Adoptive Mother's MiddleName");
            //     RuleFor(e => e.Adoption.Event.EventOwener.LastName)
            //         .Cascade(CascadeMode.StopOnFirstFailure)
            //         .NotNull()
            //         .NotEmpty()
            //         .Must((e, childLastName) => BeEqual(childLastName, e.Adoption.AdoptiveMother!.LastName)).WithMessage("the Child's lastname must be same as the Adoptive Mother's Lastname");
            // });
            RuleFor(p => p.Adoption.ApprovedName.am)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.Adoption.ApprovedName.or)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.Adoption.AdoptiveFather.FirstName.am)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.").When(p => p.Adoption.AdoptiveFather != null && p.Adoption.AdoptiveFather.FirstName != null);
            RuleFor(p => p.Adoption.AdoptiveFather.FirstName.or)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.").When(p => p.Adoption.AdoptiveFather != null && p.Adoption.AdoptiveFather.FirstName != null);
            RuleFor(p => p.Adoption.Event.EventSupportingDocuments)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.")
                .When(p => p.Adoption.Event.EventSupportingDocuments != null);
            RuleFor(e => e.Adoption.BeforeAdoptionAddressId)
                .MustAsync(ValidateForignkeyAddress)
                .WithMessage("A {PropertyName} does not  exists.")
                .When(e => e.Adoption.BeforeAdoptionAddressId != null);
            RuleFor(e => e.Adoption.Event.CivilRegOfficerId)
                .MustAsync(ValidateForignkeyPersonalInfo)
                .WithMessage("A {PropertyName} does not  exists.");
            RuleFor(e => e.Adoption.Event.CertificateId)
                .MustAsync(ValidateCertifcateId)
                .WithMessage("The last 4 digit of  {PropertyName} must be int., and must be unique.")
                .When(e => e.Adoption.Event.CertificateId != null);
            RuleFor(p => p.Adoption.Event.EventOwener.BirthDateEt)
            .MustAsync(ValidateDateEt)
               .WithMessage("{PropertyName} is invalid date.");
            RuleFor(p => p.Adoption.AdoptiveFather!.BirthDateEt)
                .MustAsync(ValidateDateEt)
                .WithMessage("{PropertyName} is invalid date.").When(p => p.Adoption.AdoptiveFather != null);

            RuleFor(p => p.Adoption.AdoptiveMother!.BirthDateEt)
                .MustAsync(ValidateDateEt)

                .WithMessage("{PropertyName} is invalid date.").When(p => p.Adoption.AdoptiveMother != null);
            // RuleFor(p => p.Adoption.Event.EventRegDateEt)
            //     .MustAsync(ValidateRegDateEt)
            //     .WithMessage("{PropertyName} is must be this year or last year.");
            RuleFor(p => p.Adoption.CourtCase.ConfirmedDateEt)
                   .MustAsync(ValidateDateEt)
                   .WithMessage("{PropertyName} is invalid date.");
            RuleFor(e => e.Adoption.AdoptiveMother!.Id)
               .MustAsync(ValidateNulleblePersonalInfoForignkey)
               .WithMessage("{PropertyName} is must be null or Person Foreign key.").When(e => e.Adoption.AdoptiveMother != null);

            RuleFor(e => e.Adoption.AdoptiveFather!.Id)
               .MustAsync(ValidateNulleblePersonalInfoForignkey)
               .WithMessage("{PropertyName} is must be null or Person Foreign key.").When(e => e.Adoption.AdoptiveMother != null);
            RuleFor(e => e.Adoption.Event.EventOwener.Id)
               .MustAsync(ValidateNulleblePersonalInfoForignkey)
               .WithMessage("{PropertyName} is must be null or Person Foreign key.");

            // RuleFor(e => e.Adoption.CourtCase.Id)
            //     .MustAsync(CheckIdOnCeate)
            //     .WithMessage("{PropertyName} is must be null on Create.");
        }
        private async Task<bool> ValidateForignkeyAddress(Guid? request, CancellationToken token)
        {
            if (request == null)
                return false;
            var address = await _address.GetByIdAsync((Guid)request);
            if (address == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task<bool> ValidateForignkeyPersonalInfo(Guid request, CancellationToken token)
        {
            var PersonalInfo = await _PersonalInfo.GetByIdAsync(request);
            if (PersonalInfo == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task<bool> ValidateNulleblePersonalInfoForignkey(Guid request, CancellationToken token)
        {
            if (request != Guid.Empty && request != null)
            {
                var PersonalInfo = await _PersonalInfo.GetByIdAsync(request);
                if (PersonalInfo == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

        }




        private async Task<bool> ValidateForignkeyLookup(Guid request, CancellationToken token)
        {
            var PersonalInfo = await _LookupsRepo.GetByIdAsync(request);
            if (PersonalInfo == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task<bool> ValidateForignkeyPaymentExamption(Guid request, CancellationToken token)
        {
            var PersonalInfo = await _PaymentExaptionRepo.GetByIdAsync(request);
            if (PersonalInfo == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task<bool> ValidateCertifcateId(string? CertId, CancellationToken token)
        {
            if (CertId == null)
                return false;
            var valid = int.TryParse(CertId.Substring(CertId.Length - 4), out _);
            if (valid)
            {
                var certfcate = _EventRepository.GetAll().Where(x => x.CertificateId == CertId && x.EventType == "Adoption").FirstOrDefault();
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

        private async Task<bool> CheckIdOnCeate(Guid? Id, CancellationToken token)
        {
            if (Id == Guid.Empty || Id == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private async Task<bool> ValidateDateEt(string? DateEt, CancellationToken token)
        {
            if (DateEt == null)
            {
                return false;
            }
            DateTime ethiodate = _dateConverter.EthiopicToGregorian(DateEt);
            if (ethiodate <= DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> ValidateRegDateEt(string DateEt, CancellationToken token)
        {


            DateTime ethiodate = _dateConverter.EthiopicToGregorian(DateEt);
            Console.WriteLine("date et {0} date now {1} reg date {2} date {3}", ethiodate.Year, DateTime.Now.Year, DateEt, ethiodate);
            if ((ethiodate.Year == DateTime.Now.Year) || (ethiodate.Year == DateTime.Now.Year - 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool BeEqual(LanguageModel obj1, LanguageModel obj2)
        {
            return obj1.am == obj2.am && obj1.en == obj2.en && obj1.or == obj2.or;
        }
    }
}




