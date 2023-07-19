

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
        public async Task job2()
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
                                    .Where(n => n.StartsWith("eventcouches")).ToList();
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
                            MarriageEventCouch marriageEventCouch = (MarriageEventCouch)eventDoc;
                            Console.WriteLine($"doc --Id-- ${marriageEventCouch.Event.Id}");

                            officerPersonalInfoId = marriageEventCouch.Event.CivilRegOfficerId;
                            uid = _userRepository.GetAll()
                                       .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                                       .Select(u => u.Id).FirstOrDefault();
                            officerUserId = string.IsNullOrEmpty(uid) ? officerUserId : new Guid(uid);
                            var marriageEventCommand = new CreateMarriageEventCommand
                            {
                                Id = marriageEventCouch.Id,
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
                            if (res.Success)
                            {
                                if (marriageEventCouch.Paid && marriageEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(marriageEventCommand.Event.Id, marriageEventCouch.Payment?.PaymentWayLookupId, marriageEventCouch.Payment?.BillNumber);
                                    //TODO:if paymentRes == false???

                                }
                                marriageEventCouch.Synced = true;
                                await eventDb.AddOrUpdateAsync(marriageEventCouch);
                            }
                            break;
                        case "birth":
                            Console.WriteLine($"doc --(_id)-- ${eventDoc.Id}");
                            BirthEventCouch birthEventCouch = (BirthEventCouch)eventDoc;

                            Console.WriteLine($"doc --Id-- ${birthEventCouch.Event.Id}");

                            officerPersonalInfoId = birthEventCouch.Event.CivilRegOfficerId;
                            uid = _userRepository.GetAll()
                                        .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                                        .Select(u => u.Id).FirstOrDefault();
                            officerUserId = string.IsNullOrEmpty(uid) ? officerUserId : new Guid(uid);

                            var birthEventCommand = new CreateBirthEventCommand
                            (new AddBirthEventRequest
                            {
                                Id = birthEventCouch.Id,
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

                            var res2 = await _mediator.Send(birthEventCommand);

                            if (res2.Success)
                            {
                                if (birthEventCouch.Paid && birthEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(birthEventCommand.BirthEvent.Event.Id, birthEventCouch.Payment?.PaymentWayLookupId, birthEventCouch.Payment?.BillNumber);
                                    //TODO:if paymentRes == false???

                                }
                                birthEventCouch.Synced = true;
                                await eventDb.AddOrUpdateAsync(birthEventCouch);
                            }


                            break;
                        case "death":
                            DeathEventCouch deathEventCouch = (DeathEventCouch)eventDoc;
                            Console.WriteLine($"doc --Id-- ${deathEventCouch.Event.Id}");

                            officerPersonalInfoId = deathEventCouch.Event.CivilRegOfficerId;
                            uid = _userRepository.GetAll()
                                          .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                                          .Select(u => u.Id).FirstOrDefault();
                            officerUserId = string.IsNullOrEmpty(uid) ? officerUserId : new Guid(uid);


                            var deathEventCommand = new CreateDeathEventCommand
                            (new AddDeathEventRequest
                            {
                                Id = deathEventCouch.Id,
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
                            //TODO:payment registration

                            if (res3.Success)
                            {
                                if (deathEventCouch.Paid && deathEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(deathEventCommand.DeathEvent.Event.Id, deathEventCouch.Payment?.PaymentWayLookupId, deathEventCouch.Payment?.BillNumber);
                                    //TODO:if paymentRes == false???

                                }
                                deathEventCouch.Synced = true;
                                await eventDb.AddOrUpdateAsync(deathEventCouch);
                            }

                            break;
                        case "adoption":
                            AdoptionEventCouch adoptionEventCouch = (AdoptionEventCouch)eventDoc;
                            Console.WriteLine($"doc --Id-- ${adoptionEventCouch.Event.Id}");

                            officerPersonalInfoId = adoptionEventCouch.Event.CivilRegOfficerId;
                            uid = _userRepository.GetAll()
                                        .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                                        .Select(u => u.Id).FirstOrDefault();
                            officerUserId = string.IsNullOrEmpty(uid) ? officerUserId : new Guid(uid);


                            var adoptionEventCommand = new CreateAdoptionCommand
                            (new AddAdoptionRequest
                            {
                                Id = adoptionEventCouch.Id,
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
                            //TODO:payment registration

                            if (res4.Success)
                            {
                                if (adoptionEventCouch.Paid && adoptionEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(adoptionEventCommand.Adoption.Event.Id, adoptionEventCouch.Payment?.PaymentWayLookupId, adoptionEventCouch.Payment?.BillNumber);
                                    //TODO:if paymentRes == false???
                                }
                                adoptionEventCouch.Synced = true;
                                await eventDb.AddOrUpdateAsync(adoptionEventCouch);
                            }


                            break;
                        case "divorce":
                            DivorceEventCouch divorceEventCouch = (DivorceEventCouch)eventDoc;
                            Console.WriteLine($"doc --Id-- ${divorceEventCouch.Event.Id}");

                            officerPersonalInfoId = divorceEventCouch.Event.CivilRegOfficerId;
                            uid = _userRepository.GetAll()
                                        .Where(u => u.PersonalInfoId == officerPersonalInfoId)
                                        .Select(u => u.Id).FirstOrDefault();
                            officerUserId = string.IsNullOrEmpty(uid) ? officerUserId : new Guid(uid);


                            var divorceEventCommand = new CreateDivorceEventCommand

                            {
                                Id = divorceEventCouch.Id,
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

                            if (res5.Success)
                            {
                                if (divorceEventCouch.Paid && divorceEventCouch.Payment != null)
                                {
                                    var paymentRes = await createPayment(divorceEventCommand.Event.Id, divorceEventCouch.Payment?.PaymentWayLookupId, divorceEventCouch.Payment?.BillNumber);
                                    //TODO:if paymentRes == false???
                                }
                                divorceEventCouch.Synced = true;
                                await eventDb.AddOrUpdateAsync(divorceEventCouch);
                            }
                            break;
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