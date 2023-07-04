using System.Text.RegularExpressions;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.Auth.ResetPassword;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Service;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.Auth.ResetPassword
{
    internal class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        private readonly ISettingRepository _settingRepository;
        private readonly HelperService _helperService;

        public ResetPasswordCommandValidator(ISettingRepository settingRepository, HelperService helperService)
        {
            _settingRepository = settingRepository;
            _helperService = helperService;
            var passwordPolicy = _helperService.getPasswordPolicySetting();
            RuleFor(u => u.resetPassword.UserName)
            .NotEmpty()
            .NotNull();
           
            RuleFor(u => u.resetPassword.Password)
              .NotNull()
              .NotEmpty()
              .Must(BeValid).WithMessage("invalid password: \n"
                +(passwordPolicy !=null?(passwordPolicy.Number?"\n password must contian number":"")
                +(passwordPolicy.LowerCase?"\nmust contian lowercase letter":"")
                +(passwordPolicy.UpperCase?"\nmust contian upper case letter":"")
                +(passwordPolicy.OtherChar?"\nmust contian special characheter":"")
                +("\nmust be maximum "+passwordPolicy.Max+" and minimum "+passwordPolicy.Min+" charachters")
                :""));
            //   .Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$").WithMessage("password must be atleast 6 digit long and must contain atlist one number one number and 1 special character")
              ;
        }

        //  number: [true, [Validators.required]],
        //   otherCharacter: [true, [Validators.required]],
        //   upperCase: [true, [Validators.required]],
        //   lowerCase: [true, [Validators.required]],
        //   minLength: [8, [Validators.required]],
        //   maxLength: [8, [Validators.required]],

        private bool BeValid(string arg)
        {
            var isValid = true;
            var passwordPolicy = _settingRepository.GetAll()
                 .Where(s => s.Key.ToLower() == "passwordpolicy")
                 .FirstOrDefault();
            if (passwordPolicy == null)
            {
                throw new NotFoundException("password policy setting not found");
            }
            if (passwordPolicy.Value.Value<bool>("number") && !arg.Any(char.IsDigit))
            {
                isValid = false;
            }
            if (passwordPolicy.Value.Value<bool>("lowerCase") && !arg.Any(char.IsLower))
            {
                isValid = false;
            }
            if (passwordPolicy.Value.Value<bool>("otherCharacter") && ! Regex.IsMatch(arg, @"[^a-zA-Z0-9]"))
            {
                isValid = false;
            }
            if (passwordPolicy.Value.Value<bool>("upperCase") && !arg.Any(char.IsUpper))
            {
                isValid = false;
            }
            if (arg.Length < passwordPolicy.Value.Value<int>("minLength"))
            {
                isValid = false;
            }
            if (arg.Length > passwordPolicy.Value.Value<int>("maxLength"))
            {
                isValid = false;
            }
            return isValid;

        }
    }
}
