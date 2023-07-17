

using AppDiv.CRVS.Application.CouchModels;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Infrastructure.Context;
using MediatR;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public class BackgroundJobs : IBackgroundJobs
    {
        private readonly CRVSCouchDbContext _couchContext;
        private readonly ISender _mediator;

        public BackgroundJobs(CRVSCouchDbContext couchContext, ISender mediator)
        {
            _couchContext = couchContext;
            _mediator = mediator;
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

                    Console.WriteLine($" got marriage db class *********   {!(marriageDb.First().Synced) && marriageDb.First().Updated == null || marriageDb.First().Updated == false}");
                }

                //Add New
                List<MarriageApplicationCouch> newMarriageApplications = marriageDb.Where(m => !(m.Synced)).ToList();
                Console.WriteLine($"new doc count ---  {newMarriageApplications.Count()} ");

                foreach (MarriageApplicationCouch marraigeApplication in newMarriageApplications)
                {
                    
                        Console.WriteLine($"creating  ---  {marraigeApplication.Id} ");
                        var marriageApplicationCommand = CustomMapper.Mapper.Map<CreateMarriageApplicationCommand>(marraigeApplication);
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

    }
}