using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using EthiopianCalendar;
using Microsoft.Extensions.Logging;

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

        public CreatAdoptionCommandValidator(IAdoptionEventRepository repo, IAddressLookupRepository address,
        IPersonalInfoRepository PersonalInfo, ILookupRepository LookupsRepo,
        IPaymentExamptionRequestRepository PaymentExaptionRepo,
        IEventRepository EventRepository
        )
        {
            _repo = repo;
            _address = address;
            _PersonalInfo = PersonalInfo;
            _LookupsRepo = LookupsRepo;
            _PaymentExaptionRepo = PaymentExaptionRepo;
            _EventRepository = EventRepository;
            RuleFor(e => e.Adoption.Id)
               .MustAsync(CheckIdOnCeate)
               .WithMessage("A {PropertyName} must be null on create.");
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
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.Adoption.AdoptiveFather.FirstName.or)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.Adoption.Event.EventSupportingDocuments)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(e => e.Adoption.BeforeAdoptionAddressId)
                .MustAsync(ValidateForignkeyAddress)
                .WithMessage("A {PropertyName} does not  exists.");
            RuleFor(e => e.Adoption.Event.EventAddressId)
                .MustAsync(ValidateForignkeyAddress)
                .WithMessage("A {PropertyName} does not  exists.");
            RuleFor(e => e.Adoption.Event.CivilRegOfficerId)
                .MustAsync(ValidateForignkeyPersonalInfo)
                .WithMessage("A {PropertyName} does not  exists.");
            RuleFor(e => e.Adoption.Event.CertificateId)
                .MustAsync(ValidateCertifcateId)
                .WithMessage("The last 4 digit of  {PropertyName} must be int., and must be unique.");
            RuleFor(p => p.Adoption.Event.EventOwener.BirthDateEt)
            .MustAsync(ValidateDateEt)
               .WithMessage("{PropertyName} is invalid date.");
            RuleFor(p => p.Adoption.AdoptiveFather.BirthDateEt)
                .MustAsync(ValidateDateEt)
                .WithMessage("{PropertyName} is invalid date.");

            RuleFor(p => p.Adoption.AdoptiveMother.BirthDateEt)
                .MustAsync(ValidateDateEt)
                .WithMessage("{PropertyName} is invalid date.");

            RuleFor(p => p.Adoption.Event.EventDateEt)
                .MustAsync(ValidateDateEt)
                .WithMessage("{PropertyName} is invalid date.");
            RuleFor(p => p.Adoption.Event.EventRegDateEt)
                .MustAsync(ValidateDateEt)
                .WithMessage("{PropertyName} is invalid date.");
            RuleFor(p => p.Adoption.CourtCase.ConfirmedDateEt)
                   .MustAsync(ValidateDateEt)
                   .WithMessage("{PropertyName} is invalid date.");
            RuleFor(e => e.Adoption.AdoptiveMother.Id)
               .MustAsync(ValidateNulleblePersonalInfoForignkey)
               .WithMessage("{PropertyName} is must be null or Person Foreign key.");

            RuleFor(e => e.Adoption.AdoptiveFather.Id)
               .MustAsync(ValidateNulleblePersonalInfoForignkey)
               .WithMessage("{PropertyName} is must be null or Person Foreign key.");
            RuleFor(e => e.Adoption.Event.EventOwener.Id)
               .MustAsync(ValidateNulleblePersonalInfoForignkey)
               .WithMessage("{PropertyName} is must be null or Person Foreign key.");

            RuleFor(e => e.Adoption.CourtCase.Id)
                .MustAsync(CheckIdOnCeate)
                .WithMessage("{PropertyName} is must be null on Create.");
        }
        private async Task<bool> ValidateForignkeyAddress(Guid request, CancellationToken token)
        {
            var address = await _address.GetByIdAsync(request);
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

        private async Task<bool> ValidateCertifcateId(string CertId, CancellationToken token)
        {
            var valid = int.TryParse(CertId.Substring(CertId.Length - 4), out _);
            if (valid)
            {
                var certfcate = _EventRepository.GetAll().Where(x => x.CertificateId == CertId).FirstOrDefault();
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


        private async Task<bool> ValidateDateEt(string DateEt, CancellationToken token)
        {
            // if (DateTime.TryParse(DateEt, out _))
            // {
            //     DateTime ethiodate = new EthiopianDate(DateTime.Parse(DateEt).Year, DateTime.Parse(DateEt).Month, DateTime.Parse(DateEt).Day).ToGregorianDate();
            //     if (ethiodate <= DateTime.Now)
            //     {
            //         return true;
            //     }
            //     else
            //     {
            //         return false;
            //     }

            // }
            // else
            // {
            //     return false;
            // }
            return false;
        }
    }
}




