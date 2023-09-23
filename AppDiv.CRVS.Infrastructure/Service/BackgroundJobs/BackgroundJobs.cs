

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
using CouchDB.Driver;
using CouchDB.Driver.DatabaseApiMethodOptions;
using CouchDB.Driver.Types;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<BackgroundJobs> logger;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly CRVSDbContext _dbContext;

        public BackgroundJobs(CRVSCouchDbContext couchContext,
                              ISender mediator,
                              IMapper mapper,
                              CRVSDbContext dbContext,
                              IUserRepository userRepository,
                              IElasticClient elasticClient,
                              ILogger<BackgroundJobs> logger,
                              IPaymentRequestRepository paymentRequestRepository)
        {
            _couchContext = couchContext;
            _mediator = mediator;
            _mapper = mapper;
            _dbContext = dbContext;
            _userRepository = userRepository;
            _elasticClient = elasticClient;
            this.logger = logger;
            _paymentRequestRepository = paymentRequestRepository;
        }
        public async Task Test()
        {
            var testDb = _couchContext.Client.GetDatabase<MarriageApplicationCouch>("test");
            var marriageAppDb = _couchContext.Client.GetDatabase<MarriageApplicationCouch>("marriageappplicationcouchesaa2b04e7-010a-11ee-a622-fa163e736d71");
            var ma = testDb.FirstOrDefault();
            // var testt = new MarriageApplicationCouch{
            //     Id = ma.Id,
            //     ApplicationDateEt = ma.ApplicationDateEt,
            //     ApplicationAddressId = ma.ApplicationAddressId,
            //     BrideInfo = ma.BrideInfo,
            //     GroomInfo = ma.GroomInfo,
            //     CivilRegOfficerId = ma.CivilRegOfficerId,
            //     Failed = ma.Failed,
            //     FailureMessage = ma.FailureMessage,
            //     Synced = ma.Synced,
            //     Certified = ma.Certified,
            //     Updated = ma.Updated,
            //     CreatedDate = ma.CreatedDate,
            //     CreatedDateGorg = ma.CreatedDateGorg,

            // };
            ma.Failed = true;
            ma.FailureMessage = "testmessage";
            var res = await testDb.AddOrUpdateAsync(ma);
            logger.LogCritical($"kkkkkkkkkkkkkkkkkkkkkkkkkkkk ===== {res}");

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
                List<MarriageApplicationCouch> newMarriageApplications = marriageDb.Where(m => !m.Synced && !m.Failed).ToList();
                Console.WriteLine($"new doc count ---  {newMarriageApplications.Count()} ");

                foreach (MarriageApplicationCouch marraigeApplication in newMarriageApplications)
                {
                    var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
                    await executionStrategy.ExecuteAsync(async () =>
                       {

                           using (var transaction = _dbContext.Database.BeginTransaction())
                           {
                               try
                               {
                                   Console.WriteLine($"creating  ---  {marraigeApplication.Id} ");
                                   var uid = _userRepository.GetAll()
                                                        .Where(u => u.PersonalInfoId == marraigeApplication.CivilRegOfficerId)
                                                        .Select(u => u.Id).FirstOrDefault();
                                   var marriageApplicationCommand
                                    = new CreateMarriageApplicationCommand
                                    {
                                        Id = new Guid(marraigeApplication.Id),
                                        ApplicationDateEt = marraigeApplication.ApplicationDateEt,
                                        ApplicationAddressId = marraigeApplication.ApplicationAddressId,
                                        BrideInfo = marraigeApplication.BrideInfo,
                                        GroomInfo = marraigeApplication.GroomInfo,
                                        CivilRegOfficerId = marraigeApplication.CivilRegOfficerId,
                                        CreatedAt = marraigeApplication.CreatedDateGorg,
                                        CreatedBy = uid != null ? new Guid(uid) : null,
                                    };

                                   var res = await _mediator.Send(marriageApplicationCommand);
                                   if (res.Success)
                                   {
                                       marraigeApplication.Synced = true;
                                       await marriageDb.AddOrUpdateAsync(marraigeApplication);
                                       await transaction.CommitAsync();
                                   }
                                   else
                                   {
                                       marraigeApplication.Failed = true;
                                       marraigeApplication.FailureMessage = res.Message;
                                       await marriageDb.AddOrUpdateAsync(marraigeApplication);
                                       await transaction.RollbackAsync();

                                   }
                                   //08db971e-eefe-431b-80ec-d74d8713817d
                                   //08db8831-cdee-438e-80a9-4100d6587b16
                               }
                               catch (Exception e)
                               {
                                   marraigeApplication.Failed = true;
                                   marraigeApplication.FailureMessage = e.Message + " /n  " + (e.InnerException != null ? e.InnerException.Message : "");
                                   var res = await marriageDb.AddOrUpdateAsync(marraigeApplication);
                                   await transaction.RollbackAsync();

                               }
                           }
                       });
                }
            }
            Console.WriteLine("job finished!!!!!!!!!!!!!!!1");
        }
        public async Task GetEventJob()
        {
            Console.WriteLine("job started event sync ....... .......");

            var eventDbNames = (await _couchContext.Client.GetDatabasesNamesAsync())
                                    .Where(n => n.StartsWith("eventcouches")).ToList();
            Console.WriteLine($"db count ---- {eventDbNames.Count}");


            foreach (string dbName in eventDbNames)
            {

                var eventDb = _couchContext.Client.GetDatabase<BaseEventCouch>(dbName);
                var unsyncedEventDocs = eventDb;
                // .Where(e => !(e.Synced) && !(e.Failed));
                Console.WriteLine($"db name ------- {dbName}");

                Console.WriteLine($"unsynced count -- {unsyncedEventDocs.ToList().Count()}");

                foreach (var eventDoc in unsyncedEventDocs)
                {
                    var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
                    await executionStrategy.ExecuteAsync(async () =>
                       {

                           using (var transaction = _dbContext.Database.BeginTransaction())
                           {
                               var eventDocCouch = eventDoc;
                               try
                               {
                                   Console.WriteLine($"doc --eventType-- ${eventDoc.EventType}");
                                //    var officerPersonalInfoId = new Guid();
                                //    string? uid = "";
                                //    var officerUserId = new Guid();

                                   switch (eventDoc.EventType.ToLower())
                                   {

                                       case "marriage":

                                           var marriageSyncRes = await marriageSync(dbName, eventDoc.Id, eventDb);
                                           eventDocCouch = marriageSyncRes.MarriageEventCouch;
                                           break;
                                       case "birth":
                                           var birthSyncRes = await birthSync(dbName, eventDoc.Id, eventDb);
                                           eventDocCouch = birthSyncRes.BirthEventCouch;
                                           break;

                                       case "death":
                                           var deathSyncRes = await deathSync(dbName, eventDoc.Id, eventDb);
                                           eventDocCouch = deathSyncRes.DeathEventCouch;
                                           break;
                                       case "adoption":
                                           var adoptionSyncRes = await adoptionSync(dbName, eventDoc.Id, eventDb);
                                           eventDocCouch = adoptionSyncRes.AdoptionEventCouch;
                                           break;
                                       case "divorce":
                                           var divorceSyncRes = await divorceSync(dbName, eventDoc.Id, eventDb);
                                           eventDocCouch = divorceSyncRes.DivorceEventCouch;
                                           break;


                                       default: break;
                                   }
                                   await transaction.CommitAsync();
                               }
                               catch (Exception e)
                               {
                                   await transaction.RollbackAsync();
                               }
                           }
                       });
                    Console.WriteLine("@@@@@@@@@@@ code after transaction @@@@@@@@@@@@@");

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
                var certificateUnsynced = eventDb.Where(e => (e.Synced) && (!e.Failed) && ((e.Certified && !e.CertificateSynced) || (e.Paid && e.paymentSynced)));

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
                            eventDocCouch.FailureMessage = "Failed to create certificate or payment for the event : \n " + e.Message + " /n  " + (e.InnerException != null ? e.InnerException.Message : "");
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

                return (false, e.Message + " /n  " + (e.InnerException != null ? e.InnerException.Message : ""));
            }


        }

        private async Task<(MarriageEventCouch? MarriageEventCouch, bool Success)> marriageSync(string dbName, string eventDocId, ICouchDatabase<BaseEventCouch>? eventDb)
        {
            MarriageEventCouch? marriageEventCouch = new MarriageEventCouch();
            var marriageDb = _couchContext.Client.GetDatabase<MarriageEventCouch>(dbName);
            try
            {
                marriageEventCouch = marriageDb.Where(b => b.Id == eventDocId).FirstOrDefault();
                // eventDocCouch = marriageEventCouch;
                Console.WriteLine($"doc --(isnull)-- ${marriageEventCouch == null}");
                Console.WriteLine($"doc --(_id)-- {eventDocId}");


                if (marriageEventCouch == null)
                {
                    // break;
                    return (MarriageEventCouch: marriageEventCouch, Success: false);
                }
                var exists = _dbContext.MarriageEvents.Where(e => e.Id == marriageEventCouch.Id2).Any();
                if (exists)
                {
                    if (marriageEventCouch.Exported)
                    {
                        marriageEventCouch.Synced = true;
                        await marriageDb.AddOrUpdateAsync(marriageEventCouch);
                        Console.WriteLine($"doc --synced -- ${marriageEventCouch.Synced}");
                        return (MarriageEventCouch: marriageEventCouch, Success: false);


                    }
                    else
                    {
                        marriageEventCouch.Failed = true;
                        marriageEventCouch.FailureMessage = $"duplicate marriage event id , marriage event with id {marriageEventCouch.Id2}already exists in database while exported flag is false";
                        await eventDb.AddOrUpdateAsync(marriageEventCouch);
                        return (MarriageEventCouch: marriageEventCouch, Success: false);

                    }
                }
                Console.WriteLine($"registering marriage event");


                Console.WriteLine($"doc --(_id)-- ${marriageEventCouch.Id}");
                Console.WriteLine($"doc --Id-- ${marriageEventCouch.Event.Id}");

                var officerPersonalInfoId = marriageEventCouch.Event.CivilRegOfficerId;
                var uid = _userRepository.GetAll()
                           .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                           .Select(u => u.Id).FirstOrDefault();
                var officerUserId = string.IsNullOrEmpty(uid) ? new Guid() : new Guid(uid);
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
                    CreatedAt = marriageEventCouch.CreatedDateGorg,
                    IsFromBgService = true
                };
                marriageEventCommand.BrideInfo.CreatedAt = marriageEventCouch.CreatedDateGorg;
                marriageEventCommand.BrideInfo.CreatedBy = officerUserId;

                marriageEventCommand.Event.CreatedAt = marriageEventCouch.CreatedDateGorg;
                marriageEventCommand.Event.CreatedBy = officerUserId;

                marriageEventCommand.Event.EventOwener.CreatedAt = marriageEventCouch.CreatedDateGorg;
                marriageEventCommand.Event.EventOwener.CreatedBy = officerUserId;
                if (marriageEventCommand.Event.PaymentExamption != null)
                {
                    marriageEventCommand.Event.PaymentExamption.CreatedAt = marriageEventCouch.CreatedDateGorg;
                    marriageEventCommand.Event.PaymentExamption.CreatedBy = officerUserId;
                }

                marriageEventCommand.Witnesses.ToList().ForEach(w =>
                {
                    w.CreatedAt = marriageEventCouch.CreatedDateGorg;
                    w.CreatedBy = officerUserId;
                    w.WitnessPersonalInfo.CreatedAt = marriageEventCouch.CreatedDateGorg;
                    w.WitnessPersonalInfo.CreatedBy = officerUserId;
                });

                var res = await _mediator.Send(marriageEventCommand);
                Console.WriteLine($"doc --save succeded-- ${res.Success}");
                Console.WriteLine($"doc --save message-- ${res.Message}");

                if (!res.Success)
                {
                    throw new Exception(res.Message);

                }


                if (marriageEventCouch.Paid && marriageEventCouch.Payment != null)
                {
                    var paymentRes = await createPayment(marriageEventCommand.Event.Id, marriageEventCouch.Payment?.PaymentWayLookupId, marriageEventCouch.Payment?.BillNumber);


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
                return (MarriageEventCouch: marriageEventCouch, Success: false);

            }
            catch (Exception e)
            {

                Console.WriteLine($"exception  ---- {e.Message}");
                if (marriageEventCouch != null)
                {
                    marriageEventCouch.Failed = true;
                    marriageEventCouch.FailureMessage = e.Message + " /n  " + (e.InnerException == null ? "" : e.InnerException.Message);
                    var res = await marriageDb.AddOrUpdateAsync(marriageEventCouch);
                }
                throw new Exception();
            }




        }

        private async Task<(BirthEventCouch? BirthEventCouch, bool Success)> birthSync(string dbName, string eventDocId, ICouchDatabase<BaseEventCouch>? eventDb)
        {
            var birthDb = _couchContext.Client.GetDatabase<BirthEventCouch>(dbName);
            var birthEventCouch = birthDb.Where(b => b.Id == eventDocId).FirstOrDefault();
            try
            {

                // eventDocCouch = birthEventCouch;
                Console.WriteLine($"doc --(isnull)-- ${birthEventCouch == null}");
                Console.WriteLine($"doc --(_id)-- {eventDocId}");


                if (birthEventCouch == null)
                {
                    return (BirthEventCouch: birthEventCouch, Success: false);
                }
                var birthExists = _dbContext.BirthEvents.Where(e => e.Id == birthEventCouch.Id2).Any();
                if (birthExists)
                {
                    if (birthEventCouch.Exported)
                    {
                        birthEventCouch.Synced = true;
                        await birthDb.AddOrUpdateAsync(birthEventCouch);
                        Console.WriteLine($"doc --synced -- ${birthEventCouch.Synced}");
                        return (BirthEventCouch: birthEventCouch, Success: false);

                    }
                    else
                    {
                        birthEventCouch.Failed = true;
                        birthEventCouch.FailureMessage = $"duplicate birth event id , birth event with id {birthEventCouch.Id2}already exists in database while exported flag is false";
                        await eventDb.AddOrUpdateAsync(birthEventCouch);
                        return (BirthEventCouch: birthEventCouch, Success: false);
                    }
                }

                Console.WriteLine($"doc --(_id)-- ${birthEventCouch.Id}");



                Console.WriteLine($"doc --Id-- ${birthEventCouch.Id2}");

                var officerPersonalInfoId = birthEventCouch.Event.CivilRegOfficerId;
                var uid = _userRepository.GetAll()
                             .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                             .Select(u => u.Id).FirstOrDefault();
                var officerUserId = string.IsNullOrEmpty(uid) ? new Guid() : new Guid(uid);

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
                    IsFromBgService = true
                });
                birthEventCommand.BirthEvent.CreatedAt = birthEventCouch.CreatedDateGorg;
                birthEventCommand.BirthEvent.CreatedBy = officerUserId;
                if (birthEventCommand.BirthEvent.Father != null)
                {
                    birthEventCommand.BirthEvent.Father.CreatedAt = birthEventCouch.CreatedDateGorg;
                    birthEventCommand.BirthEvent.Father.CreatedBy = officerUserId;
                }
                if (birthEventCommand.BirthEvent.Mother != null)
                {
                    birthEventCommand.BirthEvent.Mother.CreatedAt = birthEventCouch.CreatedDateGorg;
                    birthEventCommand.BirthEvent.Mother.CreatedBy = officerUserId;
                }
                birthEventCommand.BirthEvent.Event.CreatedAt = birthEventCouch.CreatedDateGorg;
                birthEventCommand.BirthEvent.Event.CreatedBy = officerUserId;
                birthEventCommand.BirthEvent.Event.EventOwener.CreatedAt = birthEventCouch.CreatedDateGorg;
                birthEventCommand.BirthEvent.Event.EventOwener.CreatedBy = officerUserId;

                if (birthEventCommand.BirthEvent.Event.PaymentExamption != null)
                {
                    birthEventCommand.BirthEvent.Event.PaymentExamption.CreatedAt = birthEventCouch.CreatedDateGorg;
                    birthEventCommand.BirthEvent.Event.PaymentExamption.CreatedBy = officerUserId;
                }


                if (birthEventCommand.BirthEvent.Event.EventRegistrar != null)
                {
                    birthEventCommand.BirthEvent.Event.EventRegistrar.CreatedAt = birthEventCouch.CreatedDateGorg;
                    birthEventCommand.BirthEvent.Event.EventRegistrar.CreatedBy = officerUserId;
                    birthEventCommand.BirthEvent.Event.EventRegistrar.RegistrarInfo.CreatedAt = birthEventCouch.CreatedDateGorg;
                    birthEventCommand.BirthEvent.Event.EventRegistrar.RegistrarInfo.CreatedBy = officerUserId;

                }

                if (birthEventCommand.BirthEvent.BirthNotification != null)
                {
                    birthEventCommand.BirthEvent.BirthNotification.CreatedAt = birthEventCouch.CreatedDateGorg;
                    birthEventCommand.BirthEvent.BirthNotification.CreatedBy = officerUserId;

                }

                var res2 = await _mediator.Send(birthEventCommand);
                Console.WriteLine($"doc --save succeded-- ${res2.Success}");
                Console.WriteLine($"doc --save message-- ${res2.Message}");
                if (!res2.Success)
                {
                    throw new Exception(res2.Message);

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

                return (BirthEventCouch: birthEventCouch, Success: true);
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"exception  ---- {e.Message}");
                if (birthEventCouch != null)
                {
                    birthEventCouch.Failed = true;
                    birthEventCouch.FailureMessage = e.Message + " /n  " + (e.InnerException == null ? "" : e.InnerException.Message);
                    var res = await birthDb.AddOrUpdateAsync(birthEventCouch);
                }
                throw;
            }
        }

        private async Task<(DeathEventCouch? DeathEventCouch, bool Success)> deathSync(string dbName, string eventDocId, ICouchDatabase<BaseEventCouch>? eventDb)
        {
            var deathDb = _couchContext.Client.GetDatabase<DeathEventCouch>(dbName);
            var deathEventCouch = deathDb.Where(b => b.Id == eventDocId).FirstOrDefault();
            try
            {
                // eventDocCouch = deathEventCouch;
                Console.WriteLine($"doc --(isnull)-- ${deathEventCouch == null}");
                Console.WriteLine($"doc --(_id)-- {eventDocId}");

                if (deathEventCouch == null)
                {
                    Console.WriteLine($"doc --is null ");
                    return (DeathEventCouch: deathEventCouch, Success: false);
                }
                var deathExists = _dbContext.BirthEvents.Where(e => e.Id == deathEventCouch.Id2).Any();
                if (deathExists)
                {
                    if (deathEventCouch.Exported)
                    {
                        deathEventCouch.Synced = true;
                        await deathDb.AddOrUpdateAsync(deathEventCouch);
                        Console.WriteLine($"doc --synced -- ${deathEventCouch.Synced}");
                        return (DeathEventCouch: deathEventCouch, Success: false);

                    }
                    else
                    {
                        deathEventCouch.Failed = true;
                        deathEventCouch.FailureMessage = $"duplicate birth event id , birth event with id {deathEventCouch.Id2}already exists in database while exported flag is false";
                        await eventDb.AddOrUpdateAsync(deathEventCouch);
                        return (DeathEventCouch: deathEventCouch, Success: false);
                    }
                }
                Console.WriteLine($"registering death event");

                Console.WriteLine($"doc --(_id)-- ${deathEventCouch.Id}");

                var officerPersonalInfoId = deathEventCouch.Event.CivilRegOfficerId;
                var uid = _userRepository.GetAll()
                              .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                              .Select(u => u.Id).FirstOrDefault();
                var officerUserId = string.IsNullOrEmpty(uid) ? new Guid() : new Guid(uid);


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
                    CreatedAt = deathEventCouch.CreatedDateGorg,
                    CreatedBy = officerUserId,
                    IsFromBgService = true
                });
                if (deathEventCommand.DeathEvent.DeathNotification != null)
                {
                    deathEventCommand.DeathEvent.DeathNotification.CreatedAt = deathEventCouch.CreatedDateGorg;
                    deathEventCommand.DeathEvent.DeathNotification.CreatedBy = officerUserId;

                }
                deathEventCommand.DeathEvent.Event.CreatedAt = deathEventCouch.CreatedDateGorg;
                deathEventCommand.DeathEvent.Event.CreatedBy = officerUserId;
                deathEventCommand.DeathEvent.Event.EventOwener.CreatedAt = deathEventCouch.CreatedDateGorg;
                deathEventCommand.DeathEvent.Event.EventOwener.CreatedBy = officerUserId;
                deathEventCommand.DeathEvent.Event.EventRegistrar.CreatedAt = deathEventCouch.CreatedDateGorg;
                deathEventCommand.DeathEvent.Event.EventRegistrar.CreatedBy = officerUserId;
                deathEventCommand.DeathEvent.Event.EventRegistrar.RegistrarInfo.CreatedAt = deathEventCouch.CreatedDateGorg;
                deathEventCommand.DeathEvent.Event.EventRegistrar.RegistrarInfo.CreatedBy = officerUserId;
                if (deathEventCommand.DeathEvent.Event.PaymentExamption != null)
                {
                    deathEventCommand.DeathEvent.Event.PaymentExamption.CreatedAt = deathEventCouch.CreatedDateGorg;
                    deathEventCommand.DeathEvent.Event.PaymentExamption.CreatedBy = officerUserId;
                }
                var res3 = await _mediator.Send(deathEventCommand);
                Console.WriteLine($"doc --save succeded-- ${res3.Success}");
                Console.WriteLine($"doc --save message-- ${res3.Message}");
                Console.WriteLine($"doc -  id -- ${deathEventCommand.DeathEvent.Id}");


                if (!res3.Success)
                {

                    Console.WriteLine($"doc -failed-  id -- ${deathEventCommand.DeathEvent.Id}");

                    throw new Exception(res3.Message);


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
                return (DeathEventCouch: deathEventCouch, Success: true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"exception  ---- {e.Message}");
                if (deathEventCouch != null)
                {
                    deathEventCouch.Failed = true;
                    deathEventCouch.FailureMessage = e.Message + " /n  " + (e.InnerException == null ? "" : e.InnerException.Message);
                    var res = await deathDb.AddOrUpdateAsync(deathEventCouch);
                }
                throw;
            }

        }

        private async Task<(AdoptionEventCouch? AdoptionEventCouch, bool Success)> adoptionSync(string dbName, string eventDocId, ICouchDatabase<BaseEventCouch>? eventDb)
        {
            var adoptionDb = _couchContext.Client.GetDatabase<AdoptionEventCouch>(dbName);
            var adoptionEventCouch = adoptionDb.Where(b => b.Id == eventDocId).FirstOrDefault();
            try
            {
                // eventDocCouch = adoptionEventCouch;
                Console.WriteLine($"doc --(isnull)-- ${adoptionEventCouch == null}");


                if (adoptionEventCouch == null)
                {
                    Console.WriteLine($"doc --is null ");
                    return (AdoptionEventCouch: adoptionEventCouch, Success: false);
                }
                var adoptionExists = _dbContext.BirthEvents.Where(e => e.Id == adoptionEventCouch.Id2).Any();
                if (adoptionExists)
                {
                    if (adoptionEventCouch.Exported)
                    {
                        adoptionEventCouch.Synced = true;
                        await adoptionDb.AddOrUpdateAsync(adoptionEventCouch);
                        Console.WriteLine($"doc --synced -- ${adoptionEventCouch.Synced}");
                        return (AdoptionEventCouch: adoptionEventCouch, Success: false);

                    }
                    else
                    {
                        adoptionEventCouch.Failed = true;
                        adoptionEventCouch.FailureMessage = $"duplicate birth event id , birth event with id {adoptionEventCouch.Id2}already exists in database while exported flag is false";
                        await eventDb.AddOrUpdateAsync(adoptionEventCouch);
                        return (AdoptionEventCouch: adoptionEventCouch, Success: false);
                    }
                }
                Console.WriteLine($"registering adoption event");

                Console.WriteLine($"doc --(_id)-- ${adoptionEventCouch.Id}");
                Console.WriteLine($"doc --Id-- ${adoptionEventCouch.Id2}");

                var officerPersonalInfoId = adoptionEventCouch.Event.CivilRegOfficerId;
                var uid = _userRepository.GetAll()
                            .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                            .Select(u => u.Id).FirstOrDefault();
                var officerUserId = string.IsNullOrEmpty(uid) ? new Guid() : new Guid(uid);


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
                    CreatedAt = adoptionEventCouch.CreatedDateGorg,
                    CreatedBy = officerUserId,
                    IsFromBgService = true
                });
                if (adoptionEventCommand.Adoption.AdoptiveMother != null)
                {

                    adoptionEventCommand.Adoption.AdoptiveMother.CreatedAt = adoptionEventCouch.CreatedDateGorg;
                    adoptionEventCommand.Adoption.AdoptiveMother.CreatedBy = officerUserId;
                }
                if (adoptionEventCommand.Adoption.AdoptiveFather != null)
                {

                    adoptionEventCommand.Adoption.AdoptiveFather.CreatedAt = adoptionEventCouch.CreatedDateGorg;
                    adoptionEventCommand.Adoption.AdoptiveFather.CreatedBy = officerUserId;
                }
                adoptionEventCommand.Adoption.CourtCase.CreatedAt = adoptionEventCouch.CreatedDateGorg;
                adoptionEventCommand.Adoption.CourtCase.CreatedBy = officerUserId;
                if (adoptionEventCommand.Adoption.CourtCase.Court != null)
                {
                    adoptionEventCommand.Adoption.CourtCase.Court.CreatedAt = adoptionEventCouch.CreatedDateGorg;
                    adoptionEventCommand.Adoption.CourtCase.Court.CreatedBy = officerUserId;
                }
                adoptionEventCommand.Adoption.Event.CreatedAt = adoptionEventCouch.CreatedDateGorg;
                adoptionEventCommand.Adoption.Event.CreatedBy = officerUserId;
                adoptionEventCommand.Adoption.Event.EventOwener.CreatedAt = adoptionEventCouch.CreatedDateGorg;
                adoptionEventCommand.Adoption.Event.EventOwener.CreatedBy = officerUserId;
                if (adoptionEventCommand.Adoption.Event.PaymentExamption != null)
                {
                    adoptionEventCommand.Adoption.Event.PaymentExamption.CreatedAt = adoptionEventCouch.CreatedDateGorg;
                    adoptionEventCommand.Adoption.Event.PaymentExamption.CreatedBy = officerUserId;

                }

                var res4 = await _mediator.Send(adoptionEventCommand);
                Console.WriteLine($"doc --save succeded-- ${res4.Success}");
                Console.WriteLine($"doc --save message-- ${res4.Message}");
                if (!res4.Success)
                {
                    throw new Exception(res4.Message);

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
                return (AdoptionEventCouch: adoptionEventCouch, Success: true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"exception  ---- {e.Message}");
                if (adoptionEventCouch != null)
                {
                    adoptionEventCouch.Failed = true;
                    adoptionEventCouch.FailureMessage = e.Message + " /n  " + (e.InnerException == null ? "" : e.InnerException.Message);
                    var res = await adoptionDb.AddOrUpdateAsync(adoptionEventCouch);
                }
                throw;
            }

        }

        private async Task<(DivorceEventCouch? DivorceEventCouch, bool Success)> divorceSync(string dbName, string eventDocId, ICouchDatabase<BaseEventCouch>? eventDb)
        {
            DivorceEventCouch? divorceEventCouch = new DivorceEventCouch();
            var divorceDb = _couchContext.Client.GetDatabase<DivorceEventCouch>(dbName);
            try
            {

                divorceEventCouch = divorceDb.Where(b => b.Id == eventDocId).FirstOrDefault();
                // eventDocCouch = divorceEventCouch;
                Console.WriteLine($"doc --(isnull)-- ${divorceEventCouch == null}");
                Console.WriteLine($"doc --(_id)-- {eventDocId}");


                if (divorceEventCouch == null)
                {
                    Console.WriteLine($"doc --is null ");
                    return (DivorceEventCouch: divorceEventCouch, Success: false);
                }
                var divorceExists = _dbContext.BirthEvents.Where(e => e.Id == divorceEventCouch.Id2).Any();
                if (divorceExists)
                {
                    if (divorceEventCouch.Exported)
                    {
                        divorceEventCouch.Synced = true;
                        await divorceDb.AddOrUpdateAsync(divorceEventCouch);
                        Console.WriteLine($"doc --synced -- ${divorceEventCouch.Synced}");
                        return (DivorceEventCouch: divorceEventCouch, Success: false);

                    }
                    else
                    {
                        divorceEventCouch.Failed = true;
                        divorceEventCouch.FailureMessage = $"duplicate birth event id , birth event with id {divorceEventCouch.Id2}already exists in database while exported flag is false";
                        await eventDb.AddOrUpdateAsync(divorceEventCouch);
                        return (DivorceEventCouch: divorceEventCouch, Success: false);
                    }
                }
                Console.WriteLine($"registering divorce event");

                Console.WriteLine($"doc --(_id)-- ${divorceEventCouch.Id}");
                Console.WriteLine($"doc --Id-- ${divorceEventCouch.Id2}");

                var officerPersonalInfoId = divorceEventCouch.Event.CivilRegOfficerId;
                var uid = _userRepository.GetAll()
                             .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                             .Select(u => u.Id).FirstOrDefault();
                var officerUserId = string.IsNullOrEmpty(uid) ? new Guid() : new Guid(uid);


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
                    CreatedAt = divorceEventCouch.CreatedDateGorg,
                    CreatedBy = officerUserId,
                    IsFromBgService = true
                };

                divorceEventCommand.CourtCase.CreatedAt = divorceEventCouch.CreatedDateGorg;
                divorceEventCommand.CourtCase.CreatedBy = officerUserId;
                if (divorceEventCommand.CourtCase.Court != null)
                {
                    divorceEventCommand.CourtCase.Court.CreatedAt = divorceEventCouch.CreatedDateGorg;
                    divorceEventCommand.CourtCase.Court.CreatedBy = officerUserId;
                }
                divorceEventCommand.Event.CreatedAt = divorceEventCouch.CreatedDateGorg;
                divorceEventCommand.Event.CreatedBy = officerUserId;
                divorceEventCommand.Event.EventOwener.CreatedAt = divorceEventCouch.CreatedDateGorg;
                divorceEventCommand.Event.EventOwener.CreatedBy = officerUserId;
                if (divorceEventCommand.Event.PaymentExamption != null)
                {
                    divorceEventCommand.Event.PaymentExamption.CreatedAt = divorceEventCouch.CreatedDateGorg;
                    divorceEventCommand.Event.PaymentExamption.CreatedBy = officerUserId;

                }

                var res5 = await _mediator.Send(divorceEventCommand);
                Console.WriteLine($"doc --save succeded-- ${res5.Success}");
                Console.WriteLine($"doc --save message-- ${res5.Message}");
                if (!res5.Success)
                {
                    throw new Exception(res5.Message);


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
                return (DivorceEventCouch: divorceEventCouch, Success: true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"exception  ---- {e.Message}");
                if (divorceEventCouch != null)
                {
                    divorceEventCouch.Failed = true;
                    divorceEventCouch.FailureMessage = e.Message + " /n  " + (e.InnerException == null ? "" : e.InnerException.Message);
                    var res = await divorceDb.AddOrUpdateAsync(divorceEventCouch);
                }
                throw;
            }
        }

    }
}