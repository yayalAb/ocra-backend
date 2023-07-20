

using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.CouchModels;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Create;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Create;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Features.Payments.Command.Create;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Infrastructure.Context;
using AutoMapper;
using CouchDB.Driver.Types;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public class BackgroundJobs : IBackgroundJobs
    {
        private readonly CRVSCouchDbContext _couchContext;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRequestRepository _paymentRequestRepository;

        public BackgroundJobs(CRVSCouchDbContext couchContext, ISender mediator, IMapper mapper, IUserRepository userRepository, IPaymentRequestRepository paymentRequestRepository)
        {
            _couchContext = couchContext;
            _mediator = mediator;
            _mapper = mapper;
            _userRepository = userRepository;
            _paymentRequestRepository = paymentRequestRepository;
        }
        public async Task job1()
        {
            Console.WriteLine("job start ........");
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
                         Id = marraigeApplication.Id,
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
                    }


                }

            }
            Console.WriteLine("job finished!!!!!!!!!!!!!!!1");

        }
        public async Task GetEventJob()
        {
            Console.WriteLine("job started event sync ....... .......");

            var eventDbNames = (await _couchContext.Client.GetDatabasesNamesAsync())
                                    .Where(n => n.StartsWith("eventcouchaa2b04e7-")).ToList();
            Console.WriteLine($"db count ---- {eventDbNames.Count}");


            foreach (string dbName in eventDbNames)
            {

                var eventDb = _couchContext.Client.GetDatabase<BaseEventCouch>(dbName);
                var unsyncedEventDocs = eventDb.Where(e => !(e.Synced));
                Console.WriteLine($"db name ------- {dbName}");

                Console.WriteLine($"unsynced count -- {unsyncedEventDocs.ToList().Count()}");

                foreach (var eventDoc in unsyncedEventDocs)
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
                            Console.WriteLine($"doc --(isnull)-- ${marriageEventCouch == null}");
                            Console.WriteLine($"doc --(_id)-- {eventDoc.Id}");


                            if (marriageEventCouch == null)
                            {
                                break;
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
                            try
                            {
                                var res = await _mediator.Send(marriageEventCommand);
                                Console.WriteLine($"doc --save succeded-- ${res.Success}");
                                Console.WriteLine($"doc --save message-- ${res.Message}");
                                if (res.Success)
                                {
                                    if (marriageEventCouch.Paid && marriageEventCouch.Payment != null)
                                    {
                                        var paymentRes = await createPayment(marriageEventCommand.Event.Id, marriageEventCouch.Payment?.PaymentWayLookupId, marriageEventCouch.Payment?.BillNumber);
                                        //TODO:if paymentRes == false???
                                        Console.WriteLine($"doc --payment created -- ${paymentRes}");


                                    }
                                    marriageEventCouch.Synced = true;
                                    await eventDb.AddOrUpdateAsync(marriageEventCouch);
                                    Console.WriteLine($"doc --synced -- ${marriageEventCouch.Synced}");

                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Exception {e.Message}");
                                throw;
                            }

                            break;
                        case "birth":
                            var birthDb = _couchContext.Client.GetDatabase<BirthEventCouch>(dbName);
                            var birthEventCouch = birthDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                            Console.WriteLine($"doc --(isnull)-- ${birthEventCouch == null}");
                            Console.WriteLine($"doc --(_id)-- {eventDoc.Id}");


                            if (birthEventCouch == null)
                            {
                                break;
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
                            birthEventCommand.BirthEvent.Father.CreatedAt = birthEventCouch.CreatedDate;
                            birthEventCommand.BirthEvent.Father.CreatedBy = officerUserId;
                            birthEventCommand.BirthEvent.Mother.CreatedAt = birthEventCouch.CreatedDate;
                            birthEventCommand.BirthEvent.Mother.CreatedBy = officerUserId;
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
                            try
                            {
                                var res2 = await _mediator.Send(birthEventCommand);
                                Console.WriteLine($"doc --save succeded-- ${res2.Success}");
                                Console.WriteLine($"doc --save message-- ${res2.Message}");

                                if (res2.Success)
                                {
                                    if (birthEventCouch.Paid && birthEventCouch.Payment != null)
                                    {
                                        var paymentRes = await createPayment(birthEventCommand.BirthEvent.Event.Id, birthEventCouch.Payment?.PaymentWayLookupId, birthEventCouch.Payment?.BillNumber);
                                        //TODO:if paymentRes == false???
                                        Console.WriteLine($"doc --payment created -- ${paymentRes}");


                                    }
                                    birthEventCouch.Synced = true;
                                    await eventDb.AddOrUpdateAsync(birthEventCouch);
                                    Console.WriteLine($"doc --synced -- ${birthEventCouch.Synced}");



                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"doc --exception-- ${e.Message}");
                                throw;


                            }
                            break;
                        case "death":
                            var deathDb = _couchContext.Client.GetDatabase<DeathEventCouch>(dbName);
                            var deathEventCouch = deathDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                            Console.WriteLine($"doc --(isnull)-- ${deathEventCouch == null}");
                            Console.WriteLine($"doc --(_id)-- {eventDoc.Id}");


                            if (deathEventCouch == null)
                            {
                                Console.WriteLine($"doc --is null ");
                                break;
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
                            try
                            {
                                var res3 = await _mediator.Send(deathEventCommand);
                                Console.WriteLine($"doc --save succeded-- ${res3.Success}");
                                Console.WriteLine($"doc --save message-- ${res3.Message}");

                                if (res3.Success)
                                {
                                    if (deathEventCouch.Paid && deathEventCouch.Payment != null)
                                    {
                                        var paymentRes = await createPayment(deathEventCommand.DeathEvent.Event.Id, deathEventCouch.Payment?.PaymentWayLookupId, deathEventCouch.Payment?.BillNumber);
                                        //TODO:if paymentRes == false???
                                        Console.WriteLine($"doc --payment created -- ${paymentRes}");


                                    }
                                    deathEventCouch.Synced = true;
                                    await eventDb.AddOrUpdateAsync(deathEventCouch);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"exception ----- {e.Message}");
                                throw;
                            }


                            break;
                        case "adoption":
                            var adoptionDb = _couchContext.Client.GetDatabase<AdoptionEventCouch>(dbName);
                            var adoptionEventCouch = adoptionDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                            Console.WriteLine($"doc --(isnull)-- ${adoptionEventCouch == null}");


                            if (adoptionEventCouch == null)
                            {
                                Console.WriteLine($"doc --is null ");
                                break;
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
                            
                            try
                            {
                                var res4 = await _mediator.Send(adoptionEventCommand);
                                Console.WriteLine($"doc --save succeded-- ${res4.Success}");
                                Console.WriteLine($"doc --save message-- ${res4.Message}");

                                if (res4.Success)
                                {
                                    if (adoptionEventCouch.Paid && adoptionEventCouch.Payment != null)
                                    {
                                        var paymentRes = await createPayment(adoptionEventCommand.Adoption.Event.Id, adoptionEventCouch.Payment?.PaymentWayLookupId, adoptionEventCouch.Payment?.BillNumber);
                                        //TODO:if paymentRes == false???
                                        Console.WriteLine($"doc --payment created -- ${paymentRes}");

                                    }
                                    adoptionEventCouch.Synced = true;
                                    await eventDb.AddOrUpdateAsync(adoptionEventCouch);
                                    Console.WriteLine($"doc --synced -- ${adoptionEventCouch.Synced}");

                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"exception ----- {e.Message}");
                                throw;
                            }



                            break;
                        case "divorce":
                            var divorceDb = _couchContext.Client.GetDatabase<DivorceEventCouch>(dbName);
                            var divorceEventCouch = divorceDb.Where(b => b.Id == eventDoc.Id).FirstOrDefault();
                            Console.WriteLine($"doc --(isnull)-- ${divorceEventCouch == null}");
                            Console.WriteLine($"doc --(_id)-- {eventDoc.Id}");


                            if (divorceEventCouch == null)
                            {
                                Console.WriteLine($"doc --is null ");
                                break;
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
                            try
                            {
                                var res5 = await _mediator.Send(divorceEventCommand);
                                Console.WriteLine($"doc --save succeded-- ${res5.Success}");
                                Console.WriteLine($"doc --save message-- ${res5.Message}");

                                if (res5.Success)
                                {
                                    if (divorceEventCouch.Paid && divorceEventCouch.Payment != null)
                                    {
                                        var paymentRes = await createPayment(divorceEventCommand.Event.Id, divorceEventCouch.Payment?.PaymentWayLookupId, divorceEventCouch.Payment?.BillNumber);
                                        //TODO:if paymentRes == false???
                                        Console.WriteLine($"doc --payment created -- ${paymentRes}");

                                    }
                                    divorceEventCouch.Synced = true;
                                    await eventDb.AddOrUpdateAsync(divorceEventCouch);
                                    Console.WriteLine($"doc --synced -- ${divorceEventCouch.Synced}");

                                }
                                break;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"exception  ---- {e.Message}");
                                throw;
                            }

                        default: break;
                    }
                }

            }
            Console.WriteLine("job ended  ....... .......");


        }



        private async Task<bool> createPayment(Guid? eventId, Guid? paymentWayLookupId, string? billNumber)
        {
            if (eventId != null && paymentWayLookupId != null && billNumber != null)
            {
                var paymentRequestId = await _paymentRequestRepository
                    .GetAll().Where(r => r.EventId == eventId)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();
                if (paymentRequestId != null)
                {
                    var res = await _mediator.Send(new CreatePaymentCommand
                    {
                        PaymentRequestId = paymentRequestId,
                        PaymentWayLookupId = (Guid)paymentWayLookupId,
                        BillNumber = billNumber
                    });
                    return res.Success;
                }
            }
            return false;


        }
    }
}