

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

        public async Task AddPersonIndex(List<PersonalInfoEntry> personalInfoEntries, string indexName)
        {
            Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$ started $$$$$$$$$$$$$$$$$$$$");
            Console.WriteLine($"=================== {personalInfoEntries.Count()}");

            if (personalInfoEntries.Any())
            {

                List<PersonalInfoIndex> addedPersonIndexes = new List<PersonalInfoIndex>();
                List<PersonalInfoIndex> updatedPersons = new List<PersonalInfoIndex>();
                List<Guid> deletedPersonIds = new List<Guid>();
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
                    else if (e.State == EntityState.Deleted && e.PersonalInfo != null)
                    {
                        deletedPersonIds.Add(e.PersonalInfo.Id);
                    }
                });
                // _elasticClient.Indices.Delete("personal_info");//TODO:remove this line
                
                _elasticClient.IndexMany<PersonalInfoIndex>(addedPersonIndexes, "personal_info");

                await _elasticClient.DeleteByQueryAsync<PersonalInfoIndex>(c =>
                            c.Index(indexName)
                               .Query(q =>
                                   q.Match(m =>
                                       m.Field(f => deletedPersonIds.Contains(f.Id))
                                       )));
                await _elasticClient.Indices.RefreshAsync(indexName);

                foreach (var personIndex in updatedPersons)
                {
                    await _elasticClient.UpdateByQueryAsync<PersonalInfoIndex>(c =>
                                 c.Index(indexName)
                                    .Query(q =>
                                        q.Match(m =>
                                            m.Field(f => f.Id == personIndex.Id)
                                            )).Script(script => script
                                     // Use the _source field in the script to set the entire document to the new one.
                                     .Source($"ctx._source = params.newDocument")
                                     .Params(p => p.Add("newDocument", personIndex))
                                 ));
                };
                await _elasticClient.Indices.RefreshAsync(indexName);

            }
            Console.WriteLine("################ ended #######################");

        }
        private PersonalInfoIndex mapPersonalInfoIndex(PersonalInfo personalInfo)
        {
            return new PersonalInfoIndex
            {
                Id = personalInfo.Id,
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
                GenderStr = personalInfo.SexLookup?.ValueStr,
                TypeOfWorkStr = personalInfo.TypeOfWorkLookup?.ValueStr,
                TitleStr = personalInfo.TitleLookup?.ValueStr,
                MarriageStatusStr = personalInfo.MarraigeStatusLookup?.ValueStr,
                AddressOr = personalInfo.ResidentAddress?.AddressName == null ? null : personalInfo.ResidentAddress.AddressName.Value<string>("or"),
                AddressAm = personalInfo.ResidentAddress?.AddressName == null ? null : personalInfo.ResidentAddress.AddressName.Value<string>("am"),
                DeathStatus = personalInfo.DeathStatus
            };
        }
    }
}