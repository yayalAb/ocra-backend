

using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.CouchModels;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Create;
using AppDiv.CRVS.Application.Features.Certificates.Query;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Create;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Features.Payments.Command.Create;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Infrastructure.Context;
using AutoMapper;
using CouchDB.Driver.Types;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public class BackgroundJobs : IBackgroundJobs
    {
        private readonly CRVSCouchDbContext _couchContext;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IElasticClient _elasticClient;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly CRVSDbContext _dbContext;

        public BackgroundJobs(CRVSCouchDbContext couchContext,
                              ISender mediator,
                              IMapper mapper,
                              CRVSDbContext dbContext,
                              IUserRepository userRepository,
                              IElasticClient elasticClient,
                              IPaymentRequestRepository paymentRequestRepository)
        {
            _couchContext = couchContext;
            _mediator = mediator;
            _mapper = mapper;
            _dbContext = dbContext;
            _userRepository = userRepository;
            _elasticClient = elasticClient;
            _paymentRequestRepository = paymentRequestRepository;
        }
        public async Task SyncMarriageApplicationJob()
        {
            Console.WriteLine("job started marriageApplication sync ....... .......");
            var marriageDbNames = (await _couchContext.Client.GetDatabasesNamesAsync())
                                        .Where(n => n.StartsWith("marriageappplicationcouches"));
            foreach (string marriageDbName in marriageDbNames)
            {
                Console.WriteLine($"operation for Db ---  {marriageDbName} ");

                var marriageDb = _couchContext.Client.GetDatabase<MarriageApplicationCouch>(marriageDbName);
                if (marriageDb.ToList().Count != 0)
                {

                    Console.WriteLine($" got marriage db class *********   {!(marriageDb.First().Synced)}");
                }

                //Add New
                List<MarriageApplicationCouch> newMarriageApplications = marriageDb.Where(m => !(m.Synced)).ToList();
                Console.WriteLine($"new doc count ---  {newMarriageApplications.Count()} ");

                foreach (MarriageApplicationCouch marraigeApplication in newMarriageApplications)
                {

                    Console.WriteLine($"creating  ---  {marraigeApplication.Id} ");
                    var marriageApplicationCommand
                     = new CreateMarriageApplicationCommand
                     {
                         Id = new Guid(marraigeApplication.Id),
                         ApplicationDateEt = marraigeApplication.ApplicationDateEt,
                         ApplicationAddressId = marraigeApplication.ApplicationAddressId,
                         BrideInfo = marraigeApplication.BrideInfo,
                         GroomInfo = marraigeApplication.GroomInfo,
                         CivilRegOfficerId = marraigeApplication.CivilRegOfficerId
                     };

                    var res = await _mediator.Send(marriageApplicationCommand);
                    if (res.Success)
                    {
                        marraigeApplication.Synced = true;
                        await marriageDb.AddOrUpdateAsync(marraigeApplication);
                    }//08db971e-eefe-431b-80ec-d74d8713817d
                    //08db8831-cdee-438e-80a9-4100d6587b16


                }

            }
            Console.WriteLine("job finished!!!!!!!!!!!!!!!1");

        }
        public async Task GetEventJob()
        {
            Console.WriteLine("job started event sync ....... .......");

            var eventDbNames = (await _couchContext.Client.GetDatabasesNamesAsync())
                                    .Where(n => n.StartsWith("eventcouch")).ToList();
            Console.WriteLine($"db count ---- {eventDbNames.Count}");


            foreach (string dbName in eventDbNames)
            {

                var eventDb = _couchContext.Client.GetDatabase<BaseEventCouch>(dbName);
                var unsyncedEventDocs = eventDb.Where(e => !(e.Synced));
                Console.WriteLine($"db name ------- {dbName}");

                Console.WriteLine($"unsynced count -- {unsyncedEventDocs.ToList().Count()}");

                foreach (var eventDoc in unsyncedEventDocs)
                {
                    var eventDocCouch = eventDoc;
                    try
                    {
                        Console.WriteLine($"doc --eventType-- ${eventDoc.EventType}");
                        var officerPersonalInfoId = new Guid();
                        string? uid = "";
                        var officerUserId = new Guid();

                        switch (eventDoc.EventType.ToLower())
                        {

                            case "marriage":


                                var marriageDb = _couchContext.Client.GetDatabase<MarriageEventCouch>(dbName);
                                var marriageEventCouch = marriageDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                                eventDocCouch = marriageEventCouch;
                                Console.WriteLine($"doc --(isnull)-- ${marriageEventCouch == null}");
                                Console.WriteLine($"doc --(_id)-- {eventDoc.Id}");


                                if (marriageEventCouch == null)
                                {
                                    break;
                                }
                                var exists = _dbContext.MarriageEvents.Where(e => e.Id == marriageEventCouch.Id2).Any();
                                if (exists)
                                {
                                    if (marriageEventCouch.Exported)
                                    {
                                        marriageEventCouch.Synced = true;
                                        await marriageDb.AddOrUpdateAsync(marriageEventCouch);
                                        Console.WriteLine($"doc --synced -- ${marriageEventCouch.Synced}");
                                        break;

                                    }
                                    else
                                    {
                                        marriageEventCouch.Failed = true;
                                        marriageEventCouch.FailureMessage = $"duplicate marriage event id , marriage event with id {marriageEventCouch.Id2}already exists in database while exported flag is false";
                                        await eventDb.AddOrUpdateAsync(marriageEventCouch);
                                        break;
                                    }
                                }
                                Console.WriteLine($"registering marriage event");


                                Console.WriteLine($"doc --(_id)-- ${marriageEventCouch.Id}");
                                Console.WriteLine($"doc --Id-- ${marriageEventCouch.Event.Id}");

                                officerPersonalInfoId = marriageEventCouch.Event.CivilRegOfficerId;
                                uid = _userRepository.GetAll()
                                           .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                                           .Select(u => u.Id).FirstOrDefault();
                                officerUserId = string.IsNullOrEmpty(uid) ? officerUserId : new Guid(uid);
                                var marriageEventCommand = new CreateMarriageEventCommand
                                {
                                    Id = marriageEventCouch.Id2,
                                    MarriageTypeId = marriageEventCouch.MarriageTypeId,
                                    ApplicationId = marriageEventCouch.ApplicationId,
                                    BirthCertificateBrideId = marriageEventCouch.BirthCertificateBrideId,
                                    BirthCertificateGroomId = marriageEventCouch.BirthCertificateGroomId,
                                    HasCamera = marriageEventCouch.HasCamera,
                                    HasVideo = marriageEventCouch.HasVideo,
                                    BrideInfo = marriageEventCouch.BrideInfo,
                                    Event = marriageEventCouch.Event,
                                    Witnesses = marriageEventCouch.Witnesses,
                                    CreatedAt = marriageEventCouch.CreatedDate
                                };
                                marriageEventCommand.BrideInfo.CreatedAt = marriageEventCouch.CreatedDate;
                                marriageEventCommand.BrideInfo.CreatedBy = officerUserId;

                                marriageEventCommand.Event.CreatedAt = marriageEventCouch.CreatedDate;
                                marriageEventCommand.Event.CreatedBy = officerUserId;

                                marriageEventCommand.Event.EventOwener.CreatedAt = marriageEventCouch.CreatedDate;
                                marriageEventCommand.Event.EventOwener.CreatedBy = officerUserId;
                                if (marriageEventCommand.Event.PaymentExamption != null)
                                {
                                    marriageEventCommand.Event.PaymentExamption.CreatedAt = marriageEventCouch.CreatedDate;
                                    marriageEventCommand.Event.PaymentExamption.CreatedBy = officerUserId;
                                }

                                marriageEventCommand.Witnesses.ToList().ForEach(w =>
                                {
                                    w.CreatedAt = marriageEventCouch.CreatedDate;
                                    w.CreatedBy = officerUserId;
                                    w.WitnessPersonalInfo.CreatedAt = marriageEventCouch.CreatedDate;
                                    w.WitnessPersonalInfo.CreatedBy = officerUserId;
                                });

                                var res = await _mediator.Send(marriageEventCommand);
                                Console.WriteLine($"doc --save succeded-- ${res.Success}");
                                Console.WriteLine($"doc --save message-- ${res.Message}");

                                if (!res.Success)
                                {
                                    marriageEventCouch.Failed = true;
                                    marriageEventCouch.FailureMessage = res.Message;
                                    await marriageDb.AddOrUpdateAsync(marriageEventCouch);
                                    break;
                                }


                                if (marriageEventCouch.Paid && marriageEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(marriageEventCommand.Event.Id, marriageEventCouch.Payment?.PaymentWayLookupId, marriageEventCouch.Payment?.BillNumber);

                                    // TODO:if paymentRes == false???
                                    Console.WriteLine($"doc --payment created -- ${paymentRes}");
                                    if (!paymentRes.succeded)
                                    {
                                        marriageEventCouch.Failed = true;
                                        marriageEventCouch.FailureMessage = "Failed to create payment for the event : \n" + paymentRes.message;

                                    }
                                    else
                                    {
                                        marriageEventCouch.paymentSynced = true;

                                    }

                                }
                                if (marriageEventCouch.Certified)
                                {

                                    var certificateRes = await createCertificate((Guid)marriageEventCouch.Event.Id, marriageEventCouch.serialNo);
                                    if (certificateRes.succeded)
                                    {
                                        marriageEventCouch.CertificateSynced = true;

                                    }
                                    else
                                    {
                                        marriageEventCouch.Failed = true;
                                        marriageEventCouch.FailureMessage = "Failed to create certificate for the event : \n " + certificateRes.message;
                                    }
                                }
                                marriageEventCouch.Synced = true;
                                await marriageDb.AddOrUpdateAsync(marriageEventCouch);
                                Console.WriteLine($"doc --synced -- ${marriageEventCouch.Synced}");




                                break;
                            case "birth":
                                var birthDb = _couchContext.Client.GetDatabase<BirthEventCouch>(dbName);
                                var birthEventCouch = birthDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                                eventDocCouch = birthEventCouch;
                                Console.WriteLine($"doc --(isnull)-- ${birthEventCouch == null}");
                                Console.WriteLine($"doc --(_id)-- {eventDoc.Id}");


                                if (birthEventCouch == null)
                                {
                                    break;
                                }
                                var birthExists = _dbContext.BirthEvents.Where(e => e.Id == birthEventCouch.Id2).Any();
                                if (birthExists)
                                {
                                    if (birthEventCouch.Exported)
                                    {
                                        birthEventCouch.Synced = true;
                                        await birthDb.AddOrUpdateAsync(birthEventCouch);
                                        Console.WriteLine($"doc --synced -- ${birthEventCouch.Synced}");
                                        break;

                                    }
                                    else
                                    {
                                        birthEventCouch.Failed = true;
                                        birthEventCouch.FailureMessage = $"duplicate birth event id , birth event with id {birthEventCouch.Id2}already exists in database while exported flag is false";
                                        await eventDb.AddOrUpdateAsync(birthEventCouch);
                                        break;
                                    }
                                }

                                Console.WriteLine($"doc --(_id)-- ${birthEventCouch.Id}");



                                Console.WriteLine($"doc --Id-- ${birthEventCouch.Id2}");

                                officerPersonalInfoId = birthEventCouch.Event.CivilRegOfficerId;
                                uid = _userRepository.GetAll()
                                            .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                                            .Select(u => u.Id).FirstOrDefault();
                                officerUserId = string.IsNullOrEmpty(uid) ? officerUserId : new Guid(uid);

                                var birthEventCommand = new CreateBirthEventCommand
                                (new AddBirthEventRequest
                                {
                                    Id = birthEventCouch.Id2,
                                    FacilityTypeLookupId = birthEventCouch.FacilityTypeLookupId,
                                    FacilityLookupId = birthEventCouch.FacilityLookupId,
                                    BirthPlaceId = birthEventCouch.BirthPlaceId,
                                    TypeOfBirthLookupId = birthEventCouch.TypeOfBirthLookupId,
                                    Father = birthEventCouch.Father,
                                    Mother = birthEventCouch.Mother,
                                    Event = birthEventCouch.Event,
                                    BirthNotification = birthEventCouch.BirthNotification,
                                });
                                birthEventCommand.BirthEvent.CreatedAt = birthEventCouch.CreatedDate;
                                birthEventCommand.BirthEvent.CreatedBy = officerUserId;
                                if (birthEventCommand.BirthEvent.Father != null)
                                {
                                    birthEventCommand.BirthEvent.Father.CreatedAt = birthEventCouch.CreatedDate;
                                    birthEventCommand.BirthEvent.Father.CreatedBy = officerUserId;
                                }
                                if (birthEventCommand.BirthEvent.Mother != null)
                                {
                                    birthEventCommand.BirthEvent.Mother.CreatedAt = birthEventCouch.CreatedDate;
                                    birthEventCommand.BirthEvent.Mother.CreatedBy = officerUserId;
                                }
                                birthEventCommand.BirthEvent.Event.CreatedAt = birthEventCouch.CreatedDate;
                                birthEventCommand.BirthEvent.Event.CreatedBy = officerUserId;
                                birthEventCommand.BirthEvent.Event.EventOwener.CreatedAt = birthEventCouch.CreatedDate;
                                birthEventCommand.BirthEvent.Event.EventOwener.CreatedBy = officerUserId;

                                if (birthEventCommand.BirthEvent.Event.PaymentExamption != null)
                                {
                                    birthEventCommand.BirthEvent.Event.PaymentExamption.CreatedAt = birthEventCouch.CreatedDate;
                                    birthEventCommand.BirthEvent.Event.PaymentExamption.CreatedBy = officerUserId;
                                }


                                if (birthEventCommand.BirthEvent.Event.EventRegistrar != null)
                                {
                                    birthEventCommand.BirthEvent.Event.EventRegistrar.CreatedAt = birthEventCouch.CreatedDate;
                                    birthEventCommand.BirthEvent.Event.EventRegistrar.CreatedBy = officerUserId;
                                    birthEventCommand.BirthEvent.Event.EventRegistrar.RegistrarInfo.CreatedAt = birthEventCouch.CreatedDate;
                                    birthEventCommand.BirthEvent.Event.EventRegistrar.RegistrarInfo.CreatedBy = officerUserId;

                                }

                                if (birthEventCommand.BirthEvent.BirthNotification != null)
                                {
                                    birthEventCommand.BirthEvent.BirthNotification.CreatedAt = birthEventCouch.CreatedDate;
                                    birthEventCommand.BirthEvent.BirthNotification.CreatedBy = officerUserId;

                                }

                                var res2 = await _mediator.Send(birthEventCommand);
                                Console.WriteLine($"doc --save succeded-- ${res2.Success}");
                                Console.WriteLine($"doc --save message-- ${res2.Message}");
                                if (!res2.Success)
                                {
                                    birthEventCouch.Failed = true;
                                    birthEventCouch.FailureMessage = res2.Message;
                                    await birthDb.AddOrUpdateAsync(birthEventCouch);

                                    break;
                                }


                                if (birthEventCouch.Paid && birthEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(birthEventCommand.BirthEvent.Event.Id, birthEventCouch.Payment?.PaymentWayLookupId, birthEventCouch.Payment?.BillNumber);
                                    //TODO:if paymentRes == false???
                                    Console.WriteLine($"doc --payment created -- ${paymentRes}");
                                    if (!paymentRes.succeded)
                                    {
                                        birthEventCouch.Failed = true;
                                        birthEventCouch.FailureMessage = "Failed to create payment for the event : \n" + paymentRes.message;

                                    }
                                    else
                                    {
                                        birthEventCouch.paymentSynced = true;

                                    }


                                }
                                if (birthEventCouch.Certified)
                                {

                                    var certificateRes = await createCertificate((Guid)birthEventCouch.Event.Id, birthEventCouch.serialNo);
                                    if (certificateRes.succeded)
                                    {
                                        birthEventCouch.CertificateSynced = true;

                                    }
                                    else
                                    {
                                        birthEventCouch.Failed = true;
                                        birthEventCouch.FailureMessage = "Failed to create certificate for the event : \n " + certificateRes.message;
                                    }
                                }
                                birthEventCouch.Synced = true;
                                await birthDb.AddOrUpdateAsync(birthEventCouch);
                                Console.WriteLine($"doc --synced -- ${birthEventCouch.Synced}");


                                break;

                            case "death":
                                var deathDb = _couchContext.Client.GetDatabase<DeathEventCouch>(dbName);
                                var deathEventCouch = deathDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                                eventDocCouch = deathEventCouch;
                                Console.WriteLine($"doc --(isnull)-- ${deathEventCouch == null}");
                                Console.WriteLine($"doc --(_id)-- {eventDoc.Id}");

                                if (deathEventCouch == null)
                                {
                                    Console.WriteLine($"doc --is null ");
                                    break;
                                }
                                var deathExists = _dbContext.BirthEvents.Where(e => e.Id == deathEventCouch.Id2).Any();
                                if (deathExists)
                                {
                                    if (deathEventCouch.Exported)
                                    {
                                        deathEventCouch.Synced = true;
                                        await deathDb.AddOrUpdateAsync(deathEventCouch);
                                        Console.WriteLine($"doc --synced -- ${deathEventCouch.Synced}");
                                        break;

                                    }
                                    else
                                    {
                                        deathEventCouch.Failed = true;
                                        deathEventCouch.FailureMessage = $"duplicate birth event id , birth event with id {deathEventCouch.Id2}already exists in database while exported flag is false";
                                        await eventDb.AddOrUpdateAsync(deathEventCouch);
                                        break;
                                    }
                                }
                                Console.WriteLine($"registering death event");

                                Console.WriteLine($"doc --(_id)-- ${deathEventCouch.Id}");

                                officerPersonalInfoId = deathEventCouch.Event.CivilRegOfficerId;
                                uid = _userRepository.GetAll()
                                              .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                                              .Select(u => u.Id).FirstOrDefault();
                                officerUserId = string.IsNullOrEmpty(uid) ? officerUserId : new Guid(uid);


                                var deathEventCommand = new CreateDeathEventCommand
                                (new AddDeathEventRequest
                                {
                                    Id = deathEventCouch.Id2,
                                    FacilityTypeLookupId = deathEventCouch.FacilityTypeLookupId,
                                    FacilityLookupId = deathEventCouch.FacilityLookupId,
                                    BirthCertificateId = deathEventCouch.BirthCertificateId,
                                    DuringDeathId = deathEventCouch.DuringDeathId,
                                    DeathPlaceId = deathEventCouch.DeathPlaceId,
                                    PlaceOfFuneral = deathEventCouch.PlaceOfFuneral,
                                    Event = deathEventCouch.Event,
                                    DeathNotification = deathEventCouch.DeathNotification,
                                    CreatedAt = deathEventCouch.CreatedDate,
                                    CreatedBy = officerUserId
                                });
                                if (deathEventCommand.DeathEvent.DeathNotification != null)
                                {
                                    deathEventCommand.DeathEvent.DeathNotification.CreatedAt = deathEventCouch.CreatedDate;
                                    deathEventCommand.DeathEvent.DeathNotification.CreatedBy = officerUserId;

                                }
                                deathEventCommand.DeathEvent.Event.CreatedAt = deathEventCouch.CreatedDate;
                                deathEventCommand.DeathEvent.Event.CreatedBy = officerUserId;
                                deathEventCommand.DeathEvent.Event.EventOwener.CreatedAt = deathEventCouch.CreatedDate;
                                deathEventCommand.DeathEvent.Event.EventOwener.CreatedBy = officerUserId;
                                deathEventCommand.DeathEvent.Event.EventRegistrar.CreatedAt = deathEventCouch.CreatedDate;
                                deathEventCommand.DeathEvent.Event.EventRegistrar.CreatedBy = officerUserId;
                                deathEventCommand.DeathEvent.Event.EventRegistrar.RegistrarInfo.CreatedAt = deathEventCouch.CreatedDate;
                                deathEventCommand.DeathEvent.Event.EventRegistrar.RegistrarInfo.CreatedBy = officerUserId;
                                if (deathEventCommand.DeathEvent.Event.PaymentExamption != null)
                                {
                                    deathEventCommand.DeathEvent.Event.PaymentExamption.CreatedAt = deathEventCouch.CreatedDate;
                                    deathEventCommand.DeathEvent.Event.PaymentExamption.CreatedBy = officerUserId;
                                }
                                var res3 = await _mediator.Send(deathEventCommand);
                                Console.WriteLine($"doc --save succeded-- ${res3.Success}");
                                Console.WriteLine($"doc --save message-- ${res3.Message}");
                                Console.WriteLine($"doc -  id -- ${deathEventCommand.DeathEvent.Id}");


                                if (!res3.Success)
                                {
                                    deathEventCouch.Failed = true;
                                    deathEventCouch.FailureMessage = res3.Message;
                                    Console.WriteLine($"doc -failed-  id -- ${deathEventCommand.DeathEvent.Id}");

                                    await deathDb.AddOrUpdateAsync(deathEventCouch);

                                    break;

                                }
                                if (deathEventCouch.Paid && deathEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(deathEventCommand.DeathEvent.Event.Id, deathEventCouch.Payment?.PaymentWayLookupId, deathEventCouch.Payment?.BillNumber);
                                    //TODO:if paymentRes == false???
                                    Console.WriteLine($"doc --payment created -- ${paymentRes}");
                                    if (!paymentRes.succeded)
                                    {
                                        deathEventCouch.Failed = true;
                                        deathEventCouch.FailureMessage = "Failed to create payment for the event : \n" + paymentRes.message;

                                    }
                                    else
                                    {
                                        deathEventCouch.paymentSynced = true;

                                    }
                                }
                                if (deathEventCouch.Certified)
                                {

                                    var certificateRes = await createCertificate((Guid)deathEventCouch.Event.Id, deathEventCouch.serialNo);
                                    if (certificateRes.succeded)
                                    {
                                        deathEventCouch.CertificateSynced = true;

                                    }
                                    else
                                    {
                                        deathEventCouch.Failed = true;
                                        deathEventCouch.FailureMessage = "Failed to create certificate for the event : \n " + certificateRes.message;
                                    }
                                }
                                deathEventCouch.Synced = true;
                                await deathDb.AddOrUpdateAsync(deathEventCouch);
                                break;
                            case "adoption":
                                var adoptionDb = _couchContext.Client.GetDatabase<AdoptionEventCouch>(dbName);
                                var adoptionEventCouch = adoptionDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                                eventDocCouch = adoptionEventCouch;
                                Console.WriteLine($"doc --(isnull)-- ${adoptionEventCouch == null}");


                                if (adoptionEventCouch == null)
                                {
                                    Console.WriteLine($"doc --is null ");
                                    break;
                                }
                                var adoptionExists = _dbContext.BirthEvents.Where(e => e.Id == adoptionEventCouch.Id2).Any();
                                if (adoptionExists)
                                {
                                    if (adoptionEventCouch.Exported)
                                    {
                                        adoptionEventCouch.Synced = true;
                                        await adoptionDb.AddOrUpdateAsync(adoptionEventCouch);
                                        Console.WriteLine($"doc --synced -- ${adoptionEventCouch.Synced}");
                                        break;

                                    }
                                    else
                                    {
                                        adoptionEventCouch.Failed = true;
                                        adoptionEventCouch.FailureMessage = $"duplicate birth event id , birth event with id {adoptionEventCouch.Id2}already exists in database while exported flag is false";
                                        await eventDb.AddOrUpdateAsync(adoptionEventCouch);
                                        break;
                                    }
                                }
                                Console.WriteLine($"registering adoption event");

                                Console.WriteLine($"doc --(_id)-- ${adoptionEventCouch.Id}");
                                Console.WriteLine($"doc --Id-- ${adoptionEventCouch.Id2}");

                                officerPersonalInfoId = adoptionEventCouch.Event.CivilRegOfficerId;
                                uid = _userRepository.GetAll()
                                            .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                                            .Select(u => u.Id).FirstOrDefault();
                                officerUserId = string.IsNullOrEmpty(uid) ? officerUserId : new Guid(uid);


                                var adoptionEventCommand = new CreateAdoptionCommand
                                (new AddAdoptionRequest
                                {
                                    Id = adoptionEventCouch.Id2,
                                    BeforeAdoptionAddressId = adoptionEventCouch.BeforeAdoptionAddressId,
                                    ApprovedName = adoptionEventCouch.ApprovedName,
                                    BirthCertificateId = adoptionEventCouch.BirthCertificateId,
                                    Reason = adoptionEventCouch.Reason,
                                    AdoptiveMother = adoptionEventCouch.AdoptiveMother,
                                    AdoptiveFather = adoptionEventCouch.AdoptiveFather,
                                    Event = adoptionEventCouch.Event,
                                    CourtCase = adoptionEventCouch.CourtCase,
                                    CreatedAt = adoptionEventCouch.CreatedDate,
                                    CreatedBy = officerUserId
                                });
                                if (adoptionEventCommand.Adoption.AdoptiveMother != null)
                                {

                                    adoptionEventCommand.Adoption.AdoptiveMother.CreatedAt = adoptionEventCouch.CreatedDate;
                                    adoptionEventCommand.Adoption.AdoptiveMother.CreatedBy = officerUserId;
                                }
                                if (adoptionEventCommand.Adoption.AdoptiveFather != null)
                                {

                                    adoptionEventCommand.Adoption.AdoptiveFather.CreatedAt = adoptionEventCouch.CreatedDate;
                                    adoptionEventCommand.Adoption.AdoptiveFather.CreatedBy = officerUserId;
                                }
                                adoptionEventCommand.Adoption.CourtCase.CreatedAt = adoptionEventCouch.CreatedDate;
                                adoptionEventCommand.Adoption.CourtCase.CreatedBy = officerUserId;
                                if (adoptionEventCommand.Adoption.CourtCase.Court != null)
                                {
                                    adoptionEventCommand.Adoption.CourtCase.Court.CreatedAt = adoptionEventCouch.CreatedDate;
                                    adoptionEventCommand.Adoption.CourtCase.Court.CreatedBy = officerUserId;
                                }
                                adoptionEventCommand.Adoption.Event.CreatedAt = adoptionEventCouch.CreatedDate;
                                adoptionEventCommand.Adoption.Event.CreatedBy = officerUserId;
                                adoptionEventCommand.Adoption.Event.EventOwener.CreatedAt = adoptionEventCouch.CreatedDate;
                                adoptionEventCommand.Adoption.Event.EventOwener.CreatedBy = officerUserId;
                                if (adoptionEventCommand.Adoption.Event.PaymentExamption != null)
                                {
                                    adoptionEventCommand.Adoption.Event.PaymentExamption.CreatedAt = adoptionEventCouch.CreatedDate;
                                    adoptionEventCommand.Adoption.Event.PaymentExamption.CreatedBy = officerUserId;

                                }

                                var res4 = await _mediator.Send(adoptionEventCommand);
                                Console.WriteLine($"doc --save succeded-- ${res4.Success}");
                                Console.WriteLine($"doc --save message-- ${res4.Message}");
                                if (!res4.Success)
                                {
                                    adoptionEventCouch.Failed = true;
                                    adoptionEventCouch.FailureMessage = res4.Message;
                                    await adoptionDb.AddOrUpdateAsync(adoptionEventCouch);

                                    break;

                                }


                                if (adoptionEventCouch.Paid && adoptionEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(adoptionEventCommand.Adoption.Event.Id, adoptionEventCouch.Payment?.PaymentWayLookupId, adoptionEventCouch.Payment?.BillNumber);
                                    //TODO:if paymentRes == false???
                                    Console.WriteLine($"doc --payment created -- ${paymentRes}");
                                    if (!paymentRes.succeded)
                                    {
                                        adoptionEventCouch.Failed = true;
                                        adoptionEventCouch.FailureMessage = "Failed to create payment for the event : \n" + paymentRes.message;

                                    }
                                    else
                                    {
                                        adoptionEventCouch.paymentSynced = true;

                                    }

                                }
                                if (adoptionEventCouch.Certified)
                                {

                                    var certificateRes = await createCertificate((Guid)adoptionEventCouch.Event.Id, adoptionEventCouch.serialNo);
                                    if (certificateRes.succeded)
                                    {
                                        adoptionEventCouch.CertificateSynced = true;

                                    }
                                    else
                                    {
                                        adoptionEventCouch.Failed = true;
                                        adoptionEventCouch.FailureMessage = "Failed to create certificate for the event : \n " + certificateRes.message;
                                    }
                                }
                                adoptionEventCouch.Synced = true;
                                await adoptionDb.AddOrUpdateAsync(adoptionEventCouch);
                                Console.WriteLine($"doc --synced -- ${adoptionEventCouch.Synced}");
                                break;
                            case "divorce":
                                var divorceDb = _couchContext.Client.GetDatabase<DivorceEventCouch>(dbName);
                                var divorceEventCouch = divorceDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                                eventDocCouch = divorceEventCouch;
                                Console.WriteLine($"doc --(isnull)-- ${divorceEventCouch == null}");
                                Console.WriteLine($"doc --(_id)-- {eventDoc.Id}");


                                if (divorceEventCouch == null)
                                {
                                    Console.WriteLine($"doc --is null ");
                                    break;
                                }
                                var divorceExists = _dbContext.BirthEvents.Where(e => e.Id == divorceEventCouch.Id2).Any();
                                if (divorceExists)
                                {
                                    if (divorceEventCouch.Exported)
                                    {
                                        divorceEventCouch.Synced = true;
                                        await divorceDb.AddOrUpdateAsync(divorceEventCouch);
                                        Console.WriteLine($"doc --synced -- ${divorceEventCouch.Synced}");
                                        break;

                                    }
                                    else
                                    {
                                        divorceEventCouch.Failed = true;
                                        divorceEventCouch.FailureMessage = $"duplicate birth event id , birth event with id {divorceEventCouch.Id2}already exists in database while exported flag is false";
                                        await eventDb.AddOrUpdateAsync(divorceEventCouch);
                                        break;
                                    }
                                }
                                Console.WriteLine($"registering divorce event");

                                Console.WriteLine($"doc --(_id)-- ${divorceEventCouch.Id}");
                                Console.WriteLine($"doc --Id-- ${divorceEventCouch.Id2}");

                                officerPersonalInfoId = divorceEventCouch.Event.CivilRegOfficerId;
                                uid = _userRepository.GetAll()
                                            .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                                            .Select(u => u.Id).FirstOrDefault();
                                officerUserId = string.IsNullOrEmpty(uid) ? officerUserId : new Guid(uid);


                                var divorceEventCommand = new CreateDivorceEventCommand

                                {
                                    Id = divorceEventCouch.Id2,
                                    DivorcedWife = divorceEventCouch.DivorcedWife,
                                    WifeBirthCertificateId = divorceEventCouch.WifeBirthCertificateId,
                                    HusbandBirthCertificate = divorceEventCouch.HusbandBirthCertificate,
                                    DateOfMarriageEt = divorceEventCouch.DateOfMarriageEt,
                                    DivorceReason = divorceEventCouch.DivorceReason,
                                    CourtCase = divorceEventCouch.CourtCase,
                                    NumberOfChildren = divorceEventCouch.NumberOfChildren,
                                    Event = divorceEventCouch.Event,
                                    CreatedAt = divorceEventCouch.CreatedDate,
                                    CreatedBy = officerUserId
                                };

                                divorceEventCommand.CourtCase.CreatedAt = divorceEventCouch.CreatedDate;
                                divorceEventCommand.CourtCase.CreatedBy = officerUserId;
                                if (divorceEventCommand.CourtCase.Court != null)
                                {
                                    divorceEventCommand.CourtCase.Court.CreatedAt = divorceEventCouch.CreatedDate;
                                    divorceEventCommand.CourtCase.Court.CreatedBy = officerUserId;
                                }
                                divorceEventCommand.Event.CreatedAt = divorceEventCouch.CreatedDate;
                                divorceEventCommand.Event.CreatedBy = officerUserId;
                                divorceEventCommand.Event.EventOwener.CreatedAt = divorceEventCouch.CreatedDate;
                                divorceEventCommand.Event.EventOwener.CreatedBy = officerUserId;
                                if (divorceEventCommand.Event.PaymentExamption != null)
                                {
                                    divorceEventCommand.Event.PaymentExamption.CreatedAt = divorceEventCouch.CreatedDate;
                                    divorceEventCommand.Event.PaymentExamption.CreatedBy = officerUserId;

                                }

                                var res5 = await _mediator.Send(divorceEventCommand);
                                Console.WriteLine($"doc --save succeded-- ${res5.Success}");
                                Console.WriteLine($"doc --save message-- ${res5.Message}");
                                if (!res5.Success)
                                {
                                    divorceEventCouch.Failed = true;
                                    divorceEventCouch.FailureMessage = res5.Message;
                                    await divorceDb.AddOrUpdateAsync(divorceEventCouch);

                                    break;

                                }
                                if (divorceEventCouch.Paid && divorceEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(divorceEventCommand.Event.Id, divorceEventCouch.Payment?.PaymentWayLookupId, divorceEventCouch.Payment?.BillNumber);
                                    //TODO:if paymentRes == false???
                                    Console.WriteLine($"doc --payment created -- ${paymentRes}");
                                    if (!paymentRes.succeded)
                                    {
                                        divorceEventCouch.Failed = true;
                                        divorceEventCouch.FailureMessage = "Failed to create payment for the event : \n" + paymentRes.message;
                                    }
                                    else
                                    {
                                        divorceEventCouch.paymentSynced = true;

                                    }
                                }
                                if (divorceEventCouch.Certified)
                                {

                                    var certificateRes = await createCertificate((Guid)divorceEventCouch.Event.Id, divorceEventCouch.serialNo);
                                    if (certificateRes.succeded)
                                    {
                                        divorceEventCouch.CertificateSynced = true;

                                    }
                                    else
                                    {
                                        divorceEventCouch.Failed = true;
                                        divorceEventCouch.FailureMessage = "Failed to create certificate for the event : \n " + certificateRes.message;
                                    }
                                }
                                divorceEventCouch.Synced = true;
                                await divorceDb.AddOrUpdateAsync(divorceEventCouch);
                                Console.WriteLine($"doc --synced -- ${divorceEventCouch.Synced}");
                                break;


                            default: break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"exception  ---- {e.Message}");
                        if (eventDocCouch != null)
                        {
                            eventDocCouch.Failed = true;
                            eventDocCouch.FailureMessage = e.Message;
                            var res = await eventDb.AddOrUpdateAsync(eventDocCouch);
                        }


                    }

                }

            }
            Console.WriteLine("job ended  ....... .......");


        }

        public async Task SyncCertificatesAndPayments()
        {
            var eventDbNames = (await _couchContext.Client.GetDatabasesNamesAsync())
                                 .Where(n => n.StartsWith("eventcouch")).ToList();
            Console.WriteLine($"db count ---- {eventDbNames.Count}");



            foreach (string dbName in eventDbNames)
            {
                var eventDb = _couchContext.Client.GetDatabase<BaseEventCouch>(dbName);
                var certificateUnsynced = eventDb.Where(e => (e.Synced) && ((e.Certified && !e.CertificateSynced) || (e.Paid && e.paymentSynced)));

                foreach (var eventDoc in certificateUnsynced)
                {
                    var eventDocCouch = eventDoc;
                    try
                    {


                        switch (eventDoc.EventType.ToLower())
                        {

                            case "marriage":

                                var marriageDb = _couchContext.Client.GetDatabase<MarriageEventCouch>(dbName);
                                var marriageEventCouch = marriageDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                                eventDocCouch = marriageEventCouch;



                                if (marriageEventCouch == null || marriageEventCouch.Event.Id == null)
                                {
                                    break;
                                }

                                if (marriageEventCouch.Certified)
                                {

                                    var certificateRes = await createCertificate((Guid)marriageEventCouch.Event.Id, marriageEventCouch.serialNo);
                                    if (certificateRes.succeded)
                                    {
                                        marriageEventCouch.CertificateSynced = true;

                                    }
                                    else
                                    {
                                        marriageEventCouch.Failed = true;
                                        marriageEventCouch.FailureMessage = "Failed to create certificate for the event : \n " + certificateRes.message;
                                    }
                                }
                                if (marriageEventCouch.Paid && marriageEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(marriageEventCouch.Event.Id, marriageEventCouch.Payment?.PaymentWayLookupId, marriageEventCouch.Payment?.BillNumber);

                                    if (paymentRes.succeded)
                                    {
                                        marriageEventCouch.paymentSynced = true;

                                    }
                                    else
                                    {
                                        marriageEventCouch.Failed = true;
                                        marriageEventCouch.FailureMessage = "Failed to create payment for the event : \n " + paymentRes.message;
                                    }
                                }
                                await marriageDb.AddOrUpdateAsync(marriageEventCouch);
                                break;
                            case "birth":
                                var birthDb = _couchContext.Client.GetDatabase<BirthEventCouch>(dbName);
                                var birthEventCouch = birthDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                                eventDocCouch = birthEventCouch;
                                if (birthEventCouch == null || birthEventCouch.Event.Id == null)
                                {
                                    break;
                                }

                                if (birthEventCouch.Certified)
                                {

                                    var certificateRes = await createCertificate((Guid)birthEventCouch.Event.Id, birthEventCouch.serialNo);
                                    if (certificateRes.succeded)
                                    {
                                        birthEventCouch.CertificateSynced = true;

                                    }
                                    else
                                    {
                                        birthEventCouch.Failed = true;
                                        birthEventCouch.FailureMessage = "Failed to create certificate for the event : \n " + certificateRes.message;
                                    }
                                }
                                if (birthEventCouch.Paid && birthEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(birthEventCouch.Event.Id, birthEventCouch.Payment?.PaymentWayLookupId, birthEventCouch.Payment?.BillNumber);

                                    if (paymentRes.succeded)
                                    {
                                        birthEventCouch.paymentSynced = true;

                                    }
                                    else
                                    {
                                        birthEventCouch.Failed = true;
                                        birthEventCouch.FailureMessage = "Failed to create payment for the event : \n " + paymentRes.message;
                                    }
                                }
                                await birthDb.AddOrUpdateAsync(birthEventCouch);


                                break;

                            case "death":
                                var deathDb = _couchContext.Client.GetDatabase<DeathEventCouch>(dbName);
                                var deathEventCouch = deathDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                                eventDocCouch = deathEventCouch;

                                if (deathEventCouch == null || deathEventCouch.Event.Id == null)
                                {
                                    break;
                                }

                                if (deathEventCouch.Certified)
                                {

                                    var certificateRes = await createCertificate((Guid)deathEventCouch.Event.Id, deathEventCouch.serialNo);
                                    if (certificateRes.succeded)
                                    {
                                        deathEventCouch.CertificateSynced = true;

                                    }
                                    else
                                    {
                                        deathEventCouch.Failed = true;
                                        deathEventCouch.FailureMessage = "Failed to create certificate for the event : \n " + certificateRes.message;
                                    }
                                }
                                if (deathEventCouch.Paid && deathEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(deathEventCouch.Event.Id, deathEventCouch.Payment?.PaymentWayLookupId, deathEventCouch.Payment?.BillNumber);

                                    if (paymentRes.succeded)
                                    {
                                        deathEventCouch.paymentSynced = true;

                                    }
                                    else
                                    {
                                        deathEventCouch.Failed = true;
                                        deathEventCouch.FailureMessage = "Failed to create payment for the event : \n " + paymentRes.message;
                                    }
                                }
                                await deathDb.AddOrUpdateAsync(deathEventCouch);
                                break;
                            case "adoption":
                                var adoptionDb = _couchContext.Client.GetDatabase<AdoptionEventCouch>(dbName);
                                var adoptionEventCouch = adoptionDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                                eventDocCouch = adoptionEventCouch;
                                if (adoptionEventCouch == null || adoptionEventCouch.Event.Id == null)
                                {
                                    break;
                                }
                                if (adoptionEventCouch.Certified)
                                {

                                    var certificateRes = await createCertificate((Guid)adoptionEventCouch.Event.Id, adoptionEventCouch.serialNo);
                                    if (certificateRes.succeded)
                                    {
                                        adoptionEventCouch.CertificateSynced = true;

                                    }
                                    else
                                    {
                                        adoptionEventCouch.Failed = true;
                                        adoptionEventCouch.FailureMessage = "Failed to create certificate for the event : \n " + certificateRes.message;
                                    }
                                }
                                if (adoptionEventCouch.Paid && adoptionEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(adoptionEventCouch.Event.Id, adoptionEventCouch.Payment?.PaymentWayLookupId, adoptionEventCouch.Payment?.BillNumber);

                                    if (paymentRes.succeded)
                                    {
                                        adoptionEventCouch.paymentSynced = true;

                                    }
                                    else
                                    {
                                        adoptionEventCouch.Failed = true;
                                        adoptionEventCouch.FailureMessage = "Failed to create payment for the event : \n " + paymentRes.message;
                                    }
                                }
                                await adoptionDb.AddOrUpdateAsync(adoptionEventCouch);
                                break;
                            case "divorce":
                                var divorceDb = _couchContext.Client.GetDatabase<DivorceEventCouch>(dbName);
                                var divorceEventCouch = divorceDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                                eventDocCouch = divorceEventCouch;
                                if (divorceEventCouch == null || divorceEventCouch.Event.Id == null)
                                {
                                    break;
                                }
                                if (divorceEventCouch.Certified)
                                {

                                    var certificateRes = await createCertificate((Guid)divorceEventCouch.Event.Id, divorceEventCouch.serialNo);
                                    if (certificateRes.succeded)
                                    {
                                        divorceEventCouch.CertificateSynced = true;

                                    }
                                    else
                                    {
                                        divorceEventCouch.Failed = true;
                                        divorceEventCouch.FailureMessage = "Failed to create certificate for the event : \n " + certificateRes.message;
                                    }
                                }
                                if (divorceEventCouch.Paid && divorceEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(divorceEventCouch.Event.Id, divorceEventCouch.Payment?.PaymentWayLookupId, divorceEventCouch.Payment?.BillNumber);

                                    if (paymentRes.succeded)
                                    {
                                        divorceEventCouch.paymentSynced = true;

                                    }
                                    else
                                    {
                                        divorceEventCouch.Failed = true;
                                        divorceEventCouch.FailureMessage = "Failed to create payment for the event : \n " + paymentRes.message;
                                    }
                                }
                                await divorceDb.AddOrUpdateAsync(divorceEventCouch);
                                break;


                            default: break;
                        }

                    }
                    catch (Exception e)
                    {
                        if (eventDocCouch != null)
                        {
                            eventDocCouch.Failed = true;
                            eventDocCouch.FailureMessage = "Failed to create certificate or payment for the event : \n " + e.Message;
                            await eventDb.AddOrUpdateAsync(eventDocCouch);

                        }

                    }

                }

            }


        }



        private async Task<(bool succeded, string message)> createPayment(Guid? eventId, Guid? paymentWayLookupId, string? billNumber)
        {
            if (eventId == null || paymentWayLookupId == null || billNumber == null)
            {
                return (succeded: false, message: "null feild value in  payment object");
            }

            var paymentRequestId = await _paymentRequestRepository
                .GetAll().Where(r => r.EventId == eventId)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
            if (paymentRequestId == null)
            {
                return (succeded: false, message: "payment request for the event is not found");
            }

            var res = await _mediator.Send(new CreatePaymentCommand
            {
                PaymentRequestId = paymentRequestId,
                PaymentWayLookupId = (Guid)paymentWayLookupId,
                BillNumber = billNumber
            });
            return (res.Success, res.Message);

        }

        private async Task<(bool succeded, string message)> createCertificate(Guid eventId, string? certificateSerialNumber)
        {
            try
            {
                var res = await _mediator.Send(new GenerateCertificateQuery
                {
                    Id = eventId,
                    CertificateSerialNumber = certificateSerialNumber,
                    IsPrint = true,
                    CheckSerialNumber = false

                });
                return (true, "");
            }
            catch (System.Exception e)
            {

                return (false, e.Message);
            }


        }

    }
}