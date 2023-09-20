

using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Infrastructure.Context;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace AppDiv.CRVS.Infrastructure.Service.FireAndForgetJobs
{
    public class FireAndForgetJobs : IFireAndForgetJobs
    {
        private readonly CRVSCouchDbContext _couchContext;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IElasticClient _elasticClient;
        private readonly CRVSDbContext _dbContext;
        private readonly IPaymentRequestRepository _paymentRequestRepository;

        public FireAndForgetJobs(CRVSCouchDbContext couchContext,
                              ISender mediator,
                              IMapper mapper,
                              IUserRepository userRepository,
                              IElasticClient elasticClient,
                              CRVSDbContext dbContext,
                              IPaymentRequestRepository paymentRequestRepository)
        {
            _couchContext = couchContext;
            _mediator = mediator;
            _mapper = mapper;
            _userRepository = userRepository;
            _elasticClient = elasticClient;
            _dbContext = dbContext;
            _paymentRequestRepository = paymentRequestRepository;
        }

        public async Task IndexPersonInfo(List<PersonalInfoEntry> personalInfoEntries)
        {
            Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$ started $$$$$$$$$$$$$$$$$$$$");
            Console.WriteLine($"=================== {personalInfoEntries.Count()}");

            if (personalInfoEntries.Any())
            {

                List<PersonalInfoIndex> addedPersonIndexes = new List<PersonalInfoIndex>();
                List<PersonalInfoIndex> updatedPersons = new List<PersonalInfoIndex>();
                personalInfoEntries.ForEach(e =>
                {
                    Console.WriteLine($"-8-8-8-8-8-8- {e.PersonalInfoId}");
                    e.PersonalInfo = _dbContext.PersonalInfos
                          .Where(p => p.Id == e.PersonalInfoId)
                          .Include(p => p.ResidentAddress)
                          .Include(p => p.SexLookup)
                          .Include(p => p.TypeOfWorkLookup)
                          .Include(p => p.TitleLookup)
                          .Include(p => p.MarraigeStatusLookup)
                          .Include(p => p.Events.Where(e => e.EventType == "Marriage"))
                              .ThenInclude(e => e.MarriageEvent)
                                  .ThenInclude(m => m.MarriageType).FirstOrDefault();

                    if (e.State == EntityState.Added && e.PersonalInfo != null)
                    {
                        Console.WriteLine($"dkjfkdjfkjd --------- kjsdkf-----{e.PersonalInfo}");
                        addedPersonIndexes.Add(mapPersonalInfoIndex(e.PersonalInfo));
                    }
                    else if (e.State == EntityState.Modified && e.PersonalInfo != null)
                    {
                        updatedPersons.Add(mapPersonalInfoIndex(e.PersonalInfo));
                    }
                });

                if (addedPersonIndexes.Any())
                {

                    await _elasticClient.IndexManyAsync<PersonalInfoIndex>(addedPersonIndexes, "personal_info_second");
                }

                if (updatedPersons.Any())
                {
                    // var tasks = new List<Task>();s
                    foreach (var updatedPersonIndex in updatedPersons)
                    {
                        var updateres = await _elasticClient.UpdateAsync<PersonalInfoIndex>(DocumentPath<PersonalInfoIndex>
                            .Id(updatedPersonIndex.Id.ToString()),
                            p => p.Index("personal_info_second")
                                    .Doc(updatedPersonIndex)
                                    .Refresh(Elasticsearch.Net.Refresh.True)
                                    );
                        //     tasks.Add(task);
                    };

                }
                // await Task.WhenAll();
                await _elasticClient.Indices.RefreshAsync("personal_info_second");

            }
            Console.WriteLine("################ ended #######################");

        }
        public async Task IndexCertificate(List<CertificateEntry> certificateEntries)
        {
            Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$ certificate indexing started $$$$$$$$$$$$$$$$$$$$");
            Console.WriteLine($"=================== {certificateEntries.Count()}");

            if (certificateEntries.Any())
            {

                List<CertificateIndex> addedCertificates = new List<CertificateIndex>();
                List<CertificateIndex> updatedCertificates = new List<CertificateIndex>();
                certificateEntries.ForEach(e =>
           {
               e.Certificate = _dbContext.Certificates
                            .Where(a => a.Id == e.CertificateId)
                           .Include(a => a.Event)
                           .Include(a => a.Event.AdoptionEvent).ThenInclude(ae => ae.AdoptiveMother)
                           .Include(a => a.Event.AdoptionEvent).ThenInclude(ae => ae.AdoptiveFather)
                           .Include(a => a.Event.BirthEvent).ThenInclude(be => be.Mother)
                           .Include(a => a.Event.BirthEvent).ThenInclude(be => be.Father)
                           .Include(a => a.Event.DeathEventNavigation)
                           .Include(a => a.Event.DivorceEvent)
                           .Include(a => a.Event.MarriageEvent)
                           .Include(a => a.Event.EventOwener).ThenInclude(o => o.ResidentAddress)
                           .Include(a => a.Event.EventAddress)
                           .Include(a => a.Event.EventRegisteredAddress)
                           .Include(a => a.Event.CivilRegOfficer)
                           .FirstOrDefault();
               if (e.State == EntityState.Added && e.Certificate != null)
               {
                   addedCertificates.Add(mapCertificateIndex(e.Certificate));
               }
               else if (e.State == EntityState.Modified && e.Certificate != null)
               {
                   updatedCertificates.Add(mapCertificateIndex(e.Certificate));
               }
           });

                if (addedCertificates.Any())
                {

                    await _elasticClient.IndexManyAsync<CertificateIndex>(addedCertificates, "certificate");
                }

                if (updatedCertificates.Any())
                {
                //     var tasks = new List<Task>();
                   foreach (var updatedCertificateIndex in updatedCertificates)
                    {
                        var updateres = await _elasticClient.UpdateAsync<CertificateIndex>(DocumentPath<CertificateIndex>
                            .Id(updatedCertificateIndex.Id.ToString()),
                            p => p.Index("certificate")
                                    .Doc(updatedCertificateIndex)
                                    .Refresh(Elasticsearch.Net.Refresh.True)
                                    );
                        //     tasks.Add(task);
                    };
             

                }
                // await Task.WhenAll();
                await _elasticClient.Indices.RefreshAsync("certificate");

            }
            Console.WriteLine("################ ended #######################");

        }
        private PersonalInfoIndex mapPersonalInfoIndex(PersonalInfo personalInfo)
        {
            return new PersonalInfoIndex
            {
                Id = personalInfo.Id.ToString(),
                FirstNameStr = personalInfo.FirstNameStr,
                FirstNameOr = personalInfo.FirstName == null ? null : personalInfo.FirstName.Value<string>("or"),
                FirstNameAm = personalInfo.FirstName == null ? null : personalInfo.FirstName.Value<string>("am"),
                MiddleNameStr = personalInfo.MiddleNameStr,
                MiddleNameOr = personalInfo.MiddleName == null ? null : personalInfo.MiddleName.Value<string>("or"),
                MiddleNameAm = personalInfo.MiddleName == null ? null : personalInfo.MiddleName.Value<string>("am"),
                LastNameStr = personalInfo.LastNameStr,
                LastNameOr = personalInfo.LastName == null ? null : personalInfo.LastName.Value<string>("or"),
                LastNameAm = personalInfo.LastName == null ? null : personalInfo.LastName.Value<string>("am"),
                NationalId = personalInfo.NationalId,
                PhoneNumber = personalInfo.PhoneNumber,
                BirthDate = personalInfo.BirthDate,
                GenderOr = personalInfo.SexLookup?.Value == null ? null : personalInfo.SexLookup.Value.Value<string>("or"),
                GenderAm = personalInfo.SexLookup?.Value == null ? null : personalInfo.SexLookup.Value.Value<string>("am"),
                GenderEn = personalInfo.SexLookup?.Value == null ? null : personalInfo.SexLookup.Value.Value<string>("en"),
                GenderStr = personalInfo.SexLookup?.ValueStr,
                TypeOfWorkStr = personalInfo.TypeOfWorkLookup?.ValueStr,
                TitleStr = personalInfo.TitleLookup?.ValueStr,
                MarriageStatusStr = personalInfo.MarraigeStatusLookup?.ValueStr,
                AddressOr = personalInfo.ResidentAddress?.AddressName == null ? null : personalInfo.ResidentAddress.AddressName.Value<string>("or"),
                AddressAm = personalInfo.ResidentAddress?.AddressName == null ? null : personalInfo.ResidentAddress.AddressName.Value<string>("am"),
                DeathStatus = personalInfo.DeathStatus
            };
        }

        private CertificateIndex mapCertificateIndex(Certificate certificate)
        {
            return new CertificateIndex
            {
                Id = certificate.Id.ToString(),
                EventId = certificate.Event.Id,
                EventType = certificate.Event.EventType,
                NestedEventId = certificate.Event.EventType.ToLower() == "birth"
                                                        ? certificate.Event.BirthEvent.Id
                                                        : certificate.Event.EventType.ToLower() == "death"
                                                        ? certificate.Event.DeathEventNavigation.Id
                                                        : certificate.Event.EventType.ToLower() == "marriage"
                                                        ? certificate.Event.MarriageEvent.Id
                                                        : certificate.Event.EventType.ToLower() == "adoption"
                                                        ? certificate.Event.AdoptionEvent.Id
                                                        : certificate.Event.EventType.ToLower() == "divorce"
                                                        ? certificate.Event.DivorceEvent.Id : null,
                MotherFullNameAm = certificate.Event.EventType.ToLower() == "birth"
                                                        ?
                                                        (certificate.Event.BirthEvent.Mother?.FirstName == null ? null : certificate.Event.BirthEvent.Mother.FirstName.Value<string>("am"))
                                                        + " " + (certificate.Event.BirthEvent.Mother?.MiddleName == null ? null : certificate.Event.BirthEvent.Mother.MiddleName.Value<string>("am"))
                                                        + " " + (certificate.Event.BirthEvent.Mother?.LastName == null ? null : certificate.Event.BirthEvent.Mother.LastName.Value<string>("am"))
                                : certificate.Event.EventType.ToLower() == "adoption"
                                                        ?
                                                        (certificate.Event.AdoptionEvent.AdoptiveMother?.FirstName == null ? null : certificate.Event.AdoptionEvent.AdoptiveMother.FirstName.Value<string>("am"))
                                                        + " " + (certificate.Event.AdoptionEvent.AdoptiveMother?.MiddleName == null ? null : certificate.Event.AdoptionEvent.AdoptiveMother.MiddleName.Value<string>("am"))
                                                        + " " + (certificate.Event.AdoptionEvent.AdoptiveMother?.LastName == null ? null : certificate.Event.AdoptionEvent.AdoptiveMother.LastName.Value<string>("am"))
                                : null,
                MotherFullNameOr = certificate.Event.EventType.ToLower() == "birth"
                                                        ?
                                                        (certificate.Event.BirthEvent.Mother?.FirstName == null ? null : certificate.Event.BirthEvent.Mother.FirstName.Value<string>("or"))
                                                        + " " + (certificate.Event.BirthEvent.Mother?.MiddleName == null ? null : certificate.Event.BirthEvent.Mother.MiddleName.Value<string>("or"))
                                                        + " " + (certificate.Event.BirthEvent.Mother?.LastName == null ? null : certificate.Event.BirthEvent.Mother.LastName.Value<string>("or"))
                                : certificate.Event.EventType.ToLower() == "adoption"
                                                        ?
                                                        (certificate.Event.AdoptionEvent.AdoptiveMother?.FirstName == null ? null : certificate.Event.AdoptionEvent.AdoptiveMother.FirstName.Value<string>("or"))
                                                        + " " + (certificate.Event.AdoptionEvent.AdoptiveMother?.MiddleName == null ? null : certificate.Event.AdoptionEvent.AdoptiveMother.MiddleName.Value<string>("or"))
                                                        + " " + (certificate.Event.AdoptionEvent.AdoptiveMother?.LastName == null ? null : certificate.Event.AdoptionEvent.AdoptiveMother.LastName.Value<string>("or"))
                                : null,
                CivilRegOfficerNameAm = (certificate.Event?.CivilRegOfficer == null || certificate.Event.CivilRegOfficer.FirstName == null ? " " : certificate.Event.CivilRegOfficer.FirstName.Value<string>("am"))
                                                             + " " + (certificate.Event.CivilRegOfficer == null || certificate.Event.CivilRegOfficer.MiddleName == null ? " " : certificate.Event.CivilRegOfficer.MiddleName.Value<string>("am"))
                                                             + " " + (certificate.Event.CivilRegOfficer == null || certificate.Event.CivilRegOfficer.LastName == null ? " " : certificate.Event.CivilRegOfficer.LastName.Value<string>("am")),
                CivilRegOfficerNameOr = (certificate.Event?.CivilRegOfficer == null || certificate.Event.CivilRegOfficer.FirstName == null ? " " : certificate.Event.CivilRegOfficer.FirstName.Value<string>("or"))
                                                             + " " + (certificate.Event.CivilRegOfficer == null || certificate.Event.CivilRegOfficer.MiddleName == null ? " " : certificate.Event.CivilRegOfficer.MiddleName.Value<string>("or"))
                                                             + " " + (certificate.Event.CivilRegOfficer == null || certificate.Event.CivilRegOfficer.LastName == null ? " " : certificate.Event.CivilRegOfficer.LastName.Value<string>("or")),
                CertificateId = certificate.Event?.CertificateId,
                CertificateSerialNumber = certificate.CertificateSerialNumber,
                ContentStr = certificate.ContentStr,
                AddressAm = certificate.Event?.EventOwener?.ResidentAddress == null ? null : certificate.Event.EventOwener.ResidentAddress.AddressName.Value<string>("am"),
                AddressOr = certificate.Event?.EventOwener?.ResidentAddress == null ? null : certificate.Event.EventOwener.ResidentAddress.AddressName.Value<string>("or"),
                NationalId = certificate.Event?.EventOwener?.NationalId,
                FullNameAm = (certificate.Event?.EventOwener?.FirstName == null ? null : certificate.Event.EventOwener.FirstName.Value<string>("am"))
                                                        + " " + (certificate.Event?.EventOwener?.MiddleName == null ? null : certificate.Event.EventOwener.MiddleName.Value<string>("am"))
                                                        + " " + (certificate.Event?.EventOwener?.LastName == null ? null : certificate.Event.EventOwener.LastName.Value<string>("am")),
                FullNameOr = (certificate.Event?.EventOwener?.FirstName == null ? null : certificate.Event.EventOwener.FirstName.Value<string>("or"))
                                                        + " " + (certificate.Event?.EventOwener?.MiddleName == null ? null : certificate.Event.EventOwener.MiddleName.Value<string>("or"))
                                                        + " " + (certificate.Event?.EventOwener?.LastName == null ? null : certificate.Event.EventOwener.LastName.Value<string>("or")),
                EventAddressAm = certificate.Event?.EventAddress == null ? null : certificate.Event.EventAddress.AddressName.Value<string>("am"),
                EventAddressOr = certificate.Event?.EventAddress == null ? null : certificate.Event.EventAddress.AddressName.Value<string>("or"),
                EventRegisteredAddressId = certificate.Event?.EventRegisteredAddressId == null ? null : certificate.Event?.EventRegisteredAddressId.ToString(),
                EventRegisteredAddressAm = certificate.Event.EventRegisteredAddress == null ? null : certificate.Event.EventRegisteredAddress.AddressName.Value<string>("am"),
                EventRegisteredAddressOr = certificate.Event.EventRegisteredAddress == null ? null : certificate.Event.EventRegisteredAddress.AddressName.Value<string>("or"),
                Status = certificate.Status

            };
        }


    }
}
