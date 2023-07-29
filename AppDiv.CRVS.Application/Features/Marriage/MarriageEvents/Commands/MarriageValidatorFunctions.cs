
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.Marriage.MarriageEvents.Commands
{
    public static class MarriageValidatorFunctions
    {
        public static bool brideHasDivorceInLessThanDateLimitInSetting(Guid brideId, string eventDateEt, ISettingRepository _settingRepository, IPersonalInfoRepository _personalInfoRepo)
        {

            var brideInfo = _personalInfoRepo.GetAll()
                  .Include(p => p.DivorceWifeNavigation)
                  .ThenInclude(d => d.Event)
                  .Where(p => p.Id == brideId).FirstOrDefault();
            if (brideInfo == null)
            {
                throw new NotFoundException("Bride with the provided Id is not found");
            }
            var marriageSetting = _settingRepository.GetAll()
                  .Where(s => s.Key == "marriageSetting")
                  .FirstOrDefault();
            if (marriageSetting == null)
            {
                throw new NotFoundException("marriage setting not found");
            }
            var remarryGapLimit = marriageSetting.Value.Value<string>("divorced_bride_month_limit_for_remarrying");

            var brideLastDivorceDate = brideInfo.DivorceWifeNavigation
                      .OrderBy(d => d.Event.EventDate)
                      .Select(d => d.Event.EventDate).LastOrDefault();
            var eventDate = new CustomDateConverter(eventDateEt).gorgorianDate;
            if (int.TryParse(remarryGapLimit, out int result))
            {
                return brideLastDivorceDate != null && HelperService.GetMonthDifference(brideLastDivorceDate, eventDate) < result;
            }
            else
            {
                return brideLastDivorceDate != null && HelperService.GetMonthDifference(brideLastDivorceDate, eventDate) < 6;
            }
        }
        public static bool hasSupportingDoc(ICollection<AddSupportingDocumentRequest> supportingDocs, ILookupRepository _lookupRepo, string type)
        {
            type = type.ToLower();
            var supportingDocTypeLookupId = _lookupRepo
                                            .GetAll()
                                            .Where(l => l.Key.ToLower() == "supporting-document-type"
                                                && l.ValueStr.ToLower().Contains(type))
                                            .Select(l => l.Id)
                                            .FirstOrDefault();
            if (supportingDocTypeLookupId == null)
            {
                throw new NotFoundException($"{type} supporting document type lookup is not found in database");
            }
            return supportingDocs.Where(s => s.Type == supportingDocTypeLookupId).Any();
        }

      public static bool IsAboveTheAgeLimit(string birthDate, string eventDate, bool isBride , ISettingRepository _settingRepository)
        {
            DateTime birthDateConverted = new CustomDateConverter(birthDate).gorgorianDate;
            DateTime eventDateConverted = new CustomDateConverter(eventDate).gorgorianDate;

            var marriageSetting = _settingRepository.GetAll()
                    .Where(s => s.Key == "marriageSetting")
                    .FirstOrDefault();
            if (marriageSetting == null)
            {
                throw new NotFoundException("marriage setting not found");
            }
            var ageLimit = isBride
                            ? marriageSetting.Value.Value<string>("bride_min_age")
                            : marriageSetting.Value.Value<string>("groom_min_age");
            if (ageLimit == null)
            {
                throw new NotFoundException("marriage miminum age setting not found");
            }
            if (!int.TryParse(ageLimit, out int result))
            {
                throw new InvalidCastException("invalid marriage minimum age limit setting ");

            }
            return eventDateConverted.Year - birthDateConverted.Year >= int.Parse(ageLimit);
        }

    }
}