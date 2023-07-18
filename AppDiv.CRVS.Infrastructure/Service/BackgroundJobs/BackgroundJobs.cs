

using AppDiv.CRVS.Application.CouchModels;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Infrastructure.Context;
using AutoMapper;
using CouchDB.Driver.Types;
using MediatR;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public class BackgroundJobs : IBackgroundJobs
    {
        private readonly CRVSCouchDbContext _couchContext;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public BackgroundJobs(CRVSCouchDbContext couchContext, ISender mediator, IMapper mapper)
        {
            _couchContext = couchContext;
            _mediator = mediator;
            _mapper = mapper;
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
            var eventDbNames = (await _couchContext.Client.GetDatabasesNamesAsync())
                                    .Where(n => n.StartsWith("eventCouches")).ToList();
            foreach (string dbName in eventDbNames)
            {

                var eventDb = _couchContext.Client.GetDatabase<BaseEventCouch>(dbName);
                var unsyncedEventDocs = eventDb.Where(e => !(e.Synced));
                foreach (var eventDoc in unsyncedEventDocs)
                {
                    switch (eventDoc.EventType.ToLower())
                    {
                        case "marriage":
                            MarriageEventCouch marriageEventCouch = (MarriageEventCouch)eventDoc;
                            if (marriageEventCouch.Updated != null && marriageEventCouch.Updated == true)
                            {
                                // var updateMarriageEvent = new UpdateMarriageApplicationCommand
                                // {
                                //     Id = 
                                // };
                            }
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
                            };
                            var res = await _mediator.Send(marriageEventCommand);
                            if (res.Success)
                            {
                                marriageEventCouch.Synced = true;
                                await eventDb.AddOrUpdateAsync(marriageEventCouch);

                            }
                            break;
                        case "birth":
                            break;
                        case "death":
                            break;
                        case "adoption":
                            break;
                        case "divorce":
                            break;
                        default: break;
                    }
                }

            }

        }

    }
}