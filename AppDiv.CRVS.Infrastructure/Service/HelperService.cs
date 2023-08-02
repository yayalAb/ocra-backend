

using System.Reflection;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Service;
using AppDiv.CRVS.Infrastructure.Service.FireAndForgetJobs;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Nest;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure.Services
{
    public class HelperService

    {
        private readonly CRVSDbContext _context;

        public HelperService(CRVSDbContext context)
        {
            _context = context;
        }



        public JObject getPasswordPolicy()
        {

            var policy = _context.Settings.Where(s => s.Key.ToLower() == "passwordpolicy")
                    .Select(s => s.Value).FirstOrDefault();
            if (policy == null)
            {
                throw new NotFoundException("password policy setting not found");
            }
            return policy;
        }

        public static T UpdateObjectFeilds<T>(T personalInfo, Dictionary<string, object> fieldValues)
        {
            foreach (var fieldValue in fieldValues)
            {

                string propertyName = fieldValue.Key;
                object newValue = fieldValue.Value;

                // Use reflection to get the PropertyInfo for the property
                PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName);

                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    // Update the property value dynamically
                    propertyInfo.SetValue(personalInfo, newValue);

                }
                else
                {
                    throw new Exception($"invalid feildname '{propertyName}'");
                }
            }
            return personalInfo;
        }
        public static int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;

            // Check if the current date is before the birth date in the current year
            if (DateTime.Now.Month < birthDate.Month || (DateTime.Now.Month == birthDate.Month && DateTime.Now.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }
        public static string getCurrentLanguage()
        {
            var lang = "or";
            var httpContext = new HttpContextAccessor().HttpContext;
            if (httpContext != null && httpContext.Request.Headers.ContainsKey("lang"))
            {
                httpContext.Request.Headers.TryGetValue("lang", out StringValues headerValue);
                lang = headerValue.FirstOrDefault();
            }

            return lang;
        }
        public static bool IsBase64String(string input)
        {
            try
            {
                var base64String = input.Substring(input.IndexOf(',') + 1);
                // Attempt to convert the input string to a byte array
                byte[] buffer = Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                // If the conversion fails, catch the FormatException and return false
                return false;
            }
        }
        public static void IndexPersonalInfo(List<PersonalInfoEntry> personalInfoEntries, CRVSDbContext _dbContext)
        {
            // List<object> addedPersons = new List<object>();
            // List<PersonalInfoIndex> addedPersonIndexes = new List<PersonalInfoIndex>();
            // List<object> updatedPersons = new List<object>();
            // List<Guid> deletedPersonIds = new List<Guid>();
            // personalInfoEntries.ForEach(e =>
            // {
            //     var p =
            //    _dbContext.PersonalInfos
            //         .Where(p => p.Id == e.PersonalInfoId)
            //         .Include(p => p.ResidentAddress)
            //         .Include(p => p.SexLookup)
            //         .Include(p => p.TypeOfWorkLookup)
            //         .Include(p => p.TitleLookup)
            //         .Include(p => p.MarraigeStatusLookup)
            //         .Include(p => p.Events.Where(e => e.EventType == "Marriage"))
            //             .ThenInclude(e => e.MarriageEvent)
            //                 .ThenInclude(m => m.MarriageType)
            //         ;
            //     e.PersonalInfo = p.FirstOrDefault();
            //     if (e.State == EntityState.Added && e.PersonalInfo != null)
            //     {
            //         addedPersons.Add(e.PersonalInfo);
            //         if (p != null)
            //         {

            //             addedPersonIndexes.Add(p.Select(personalInfo => new PersonalInfoIndex
            //             {
            //                 Id = personalInfo.Id,
            //                 FirstNameStr = personalInfo.FirstNameStr,
            //                 FirstNameOr = personalInfo.FirstName == null ? null : personalInfo.FirstName.Value<string>("or"),
            //                 FirstNameAm = personalInfo.FirstName == null ? null : personalInfo.FirstName.Value<string>("am"),
            //                 MiddleNameStr = personalInfo.MiddleNameStr,
            //                 MiddleNameOr = personalInfo.MiddleName == null ? null : personalInfo.MiddleName.Value<string>("or"),
            //                 MiddleNameAm = personalInfo.MiddleName == null ? null : personalInfo.MiddleName.Value<string>("am"),
            //                 LastNameStr = personalInfo.LastNameStr,
            //                 LastNameOr = personalInfo.LastName == null ? null : personalInfo.LastName.Value<string>("or"),
            //                 LastNameAm = personalInfo.LastName == null ? null : personalInfo.LastName.Value<string>("am"),
            //                 NationalId = personalInfo.NationalId,
            //                 PhoneNumber = personalInfo.PhoneNumber,
            //                 BirthDate = personalInfo.BirthDate,
            //                 GenderOr = personalInfo.SexLookup.Value == null ? null : personalInfo.SexLookup.Value.Value<string>("or"),
            //                 GenderAm = personalInfo.SexLookup.Value == null ? null : personalInfo.SexLookup.Value.Value<string>("am"),
            //                 GenderStr = personalInfo.SexLookup.ValueStr,
            //                 TypeOfWorkStr = personalInfo.TypeOfWorkLookup.ValueStr,
            //                 TitleStr = personalInfo.TitleLookup.ValueStr,
            //                 MarriageStatusStr = personalInfo.MarraigeStatusLookup.ValueStr,
            //                 AddressOr = personalInfo.ResidentAddress.AddressName == null ? null : personalInfo.ResidentAddress.AddressName.Value<string>("or"),
            //                 AddressAm = personalInfo.ResidentAddress.AddressName == null ? null : personalInfo.ResidentAddress.AddressName.Value<string>("am"),
            //                 DeathStatus = personalInfo.DeathStatus
            //             }).FirstOrDefault()!);
            //         }
            //     }
            //     else if (e.State == EntityState.Modified && e.PersonalInfo != null)
            //     {
            //         updatedPersons.Add(e.PersonalInfo);
            //     }
            //     else if (e.State == EntityState.Deleted && e.PersonalInfo != null)
            //     {
            //         deletedPersonIds.Add(e.PersonalInfo.Id);
            //     }
            // });
            // BackgroundJob.Schedule<IBackgroundJobs>(x => x.AddPersonIndex(addedPersonIndexes, "personal_info"),TimeSpan.Zero);
            // BackgroundJob.Enqueue<IFireAndForgetJobs>(x => x.AddPersonIndex(addedPersonIndexes, "personal_info"));
            // BackgroundJob.Enqueue<IBackgroundJobs>(x => x.RemoveIndex<PersonalInfoIndex>(deletedPersonIds, "personal_info"));


        }

        public static void IndexCertificates(List<CertificateEntry> certificateEntries, CRVSDbContext _dbContext)
        {
            // List<object> addedCertificates = new List<object>();
            // List<object> updatedCertificates = new List<object>();
            // List<Guid> deletedCertificates = new List<Guid>();
            // certificateEntries.ForEach(e =>
            // {
            //     e.Certificate = _dbContext.Certificates
            //                  .Where(a => a.Id == e.CertificateId)
            //                 .Include(a => a.Event)
            //                 .Include(a => a.Event.AdoptionEvent).ThenInclude(ae => ae.AdoptiveMother)
            //                 .Include(a => a.Event.BirthEvent)
            //                 .Include(a => a.Event.DeathEventNavigation)
            //                 .Include(a => a.Event.DivorceEvent)
            //                 .Include(a => a.Event.MarriageEvent)
            //                 .Include(a => a.Event.EventOwener).ThenInclude(o => o.ResidentAddress)
            //                 .Include(a => a.Event.EventAddress)
            //                 .Include(a => a.Event.CivilRegOfficer)
            //                 .FirstOrDefault();
            //     if (e.State == EntityState.Added && e.Certificate != null)
            //     {
            //         addedCertificates.Add(e.Certificate);
            //     }
            //     else if (e.State == EntityState.Modified && e.Certificate != null)
            //     {
            //         updatedCertificates.Add(e.Certificate);
            //     }
            //     else if (e.State == EntityState.Deleted && e.Certificate != null)
            //     {
            //         deletedCertificates.Add(e.Certificate.Id);
            //     }
            // });
            // BackgroundJob.Enqueue<IBackgroundJobs>(x => x.AddIndex<CertificateIndex>(addedCertificates, "certificates"));
            // BackgroundJob.Enqueue<IBackgroundJobs>(x => x.UpdateIndex<CertificateIndex>(updatedCertificates, "certificates"));
            // BackgroundJob.Enqueue<IBackgroundJobs>(x => x.RemoveIndex<CertificateIndex>(deletedCertificates, "certificates"));


        }



        public static void fourth()
        {


            // // List<PersonalInfo> personalInfos = entities.Select(e => (PersonalInfo)e).ToList();
            // // indexObj = mapPersonalInfoIndex(personalInfos);
            // Console.WriteLine("jkjkjkjkjkjkjkjkjkjkjkjkjkkjkjkjk");
            // Console.WriteLine($"---------------------------------------");


            // // var indexObj = entities.Select(e => (PersonalInfoIndex)e).ToList();
            // // _elasticClient.Indices.Delete("personal_info");//TODO:remove this line


            // // _elasticClient.IndexMany<PersonalInfo>((IEnumerable<PersonalInfo>)indexObj, "personal_info");
            // // await _elasticClient.Indices.RefreshAsync(indexName);

            // //TODO:indexed boolean in certificate table and bg service for indexing failed certificate indexes

        }

    }
}
