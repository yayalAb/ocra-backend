
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Features.Search;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Infrastructure.Services;
using AppDiv.CRVS.Domain.Entities;
using Nest;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class PersonalInfoRepository : BaseRepository<PersonalInfo>, IPersonalInfoRepository
    {
        private readonly CRVSDbContext dbContext;
        private readonly IElasticClient _elasticClient;

        public PersonalInfoRepository(CRVSDbContext dbContext, IElasticClient elasticClient) : base(dbContext)
        {
            this.dbContext = dbContext;
            _elasticClient = elasticClient;
        }

        public async Task<PersonalInfo> GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
        public IQueryable<PersonalInfo> GetAllQueryable()
        {
            return dbContext.PersonalInfos;
        }

        public override async Task InsertAsync(PersonalInfo person, CancellationToken cancellationToken)
        {
            await base.InsertAsync(person, cancellationToken);


        }
        public void EFUpdate(PersonalInfo personalInfo)
        {
            dbContext.PersonalInfos.Update(personalInfo);
        }
        public void Attach(PersonalInfo personalInfo)
        {
            dbContext.PersonalInfos.Attach(personalInfo);
        }

        public PersonalInfo GetById(Guid id)
        {

            return dbContext.PersonalInfos.Find(id);
        }
        public bool CheckPerson(Guid id)
        {
            var person = dbContext.PersonalInfos.Find(id);
            if (person != null)
            {
                return true;
            }
            return false;
        }

        public bool Exists(Guid id)
        {
            return dbContext.PersonalInfos.Where(p => p.Id == id).Any();
        }

        public async Task<List<PersonSearchResponse>> SearchPersonalInfo(GetPersonalInfoQuery query)
        {
            _elasticClient.Indices.Delete("personal_info");
            if (!_elasticClient.Indices.Exists("personal_info").Exists)
            {

                var indexRes = _elasticClient
                   .IndexMany<PersonalInfoIndex>(dbContext.PersonalInfos
                        .Where(p => p.DeathStatus == false)
                        
                       .Select(p => new PersonalInfoIndex
                       {
                           Id = p.Id,
                           FirstNameStr = p.FirstNameStr,
                           FirstNameOr = p.FirstName == null ? null : p.FirstName.Value<string>("or"),
                           FirstNameAm = p.FirstName == null ? null : p.FirstName.Value<string>("am"),
                           MiddleNameStr = p.MiddleNameStr,
                           MiddleNameOr = p.MiddleName == null ? null : p.MiddleName.Value<string>("or"),
                           MiddleNameAm = p.MiddleName == null ? null : p.MiddleName.Value<string>("am"),
                           LastNameStr = p.LastNameStr,
                           LastNameOr = p.LastName == null ? null : p.LastName.Value<string>("or"),
                           LastNameAm = p.LastName == null ? null : p.LastName.Value<string>("am"),
                           NationalId = p.NationalId,
                           PhoneNumber = p.PhoneNumber,
                           BirthDate = p.BirthDate,
                           GenderOr = p.SexLookup.Value == null ? null : p.SexLookup.Value.Value<string>("or"),
                           GenderAm = p.SexLookup.Value == null ? null : p.SexLookup.Value.Value<string>("am"),
                           GenderStr = p.SexLookup.ValueStr,
                           TypeOfWorkStr = p.TypeOfWorkLookup.ValueStr,
                           TitleStr = p.TitleLookup.ValueStr,
                           MarriageStatusStr = p.MarraigeStatusLookup.ValueStr,
                           AddressOr = p.ResidentAddress.AddressName == null ? null : p.ResidentAddress.AddressName.Value<string>("or"),
                           AddressAm = p.ResidentAddress.AddressName == null ? null : p.ResidentAddress.AddressName.Value<string>("am"),
                           DeathStatus = p.DeathStatus,
                           HasCivilMarriage = p.Events.Where(e =>  e.EventType.ToLower() =="marriage"  
                                // &&  (EF.Functions.Like( e.MarriageEvent.MarriageType.ValueStr, "%Seera Siivilii%")|| EF.Functions.Like( e.MarriageEvent.MarriageType.ValueStr, "%በመዘጋጃ የተመዘገቡ%"))
                                ).Any()  
                                

                       }), "personal_info");
                    await _elasticClient.Indices.RefreshAsync("personal_info");
            }
            var response = _elasticClient.SearchAsync<PersonalInfoIndex>(s => s
                    .Index("personal_info")
                    .Source(src => src
                    .Includes(i => i
                        .Fields(
                             f => f.Id,
                            f => f.FirstNameAm,
                            f => f.FirstNameOr,
                            f => f.MiddleNameAm,
                            f => f.MiddleNameOr,
                            f => f.LastNameAm,
                            f => f.LastNameOr,
                            f => f.AddressAm,
                            f => f.AddressOr,
                            f => f.NationalId,
                            f => f.DeathStatus,
                            f => f.HasCivilMarriage
                        )
                    ))
                    .Query(q =>
                    q.Bool(b =>
                     b.Must(
                         query.age != 0 ?
                        mu => mu.DateRange(r => r
                        .Field(f => f.BirthDate)
                        .LessThanOrEquals(DateTime.Now.AddYears(-query.age)
                        )) : null,
                        query.gender != "null" && query.gender != null && query.gender != string.Empty ?
                        mu => mu.Wildcard(w => w
                    .Field(f => f.GenderStr).Value($"*{query.gender}*")
                    ) : null
                        ))
                    && (
                        q
                    .Wildcard(w => w
                    .Field(f => f.FirstNameStr).Value($"*{query.SearchString}*")
                    ) ||
                     q
                    .Wildcard(w => w
                    .Field(f => f.MiddleNameStr).Value($"*{query.SearchString}*")
                    ) ||
                     q
                    .Wildcard(w => w
                    .Field(f => f.LastNameStr).Value($"*{query.SearchString}*")
                    ) ||
                     q
                    .Wildcard(w => w
                    .Field(f => f.NationalId).Value($"*{query.SearchString}*")
                    ) ||
                     q
                    .Wildcard(w => w
                    .Field(f => f.GenderStr).Value($"*{query.SearchString}*")
                    ) ||
                     q
                    .Wildcard(w => w
                    .Field(f => f.GenderStr).Value($"*{query.SearchString}*")
                    ) ||
                    q
                    .Wildcard(w => w
                    .Field(f => f.TypeOfWorkStr).Value($"*{query.SearchString}*")
                    ) ||
                    q
                    .Wildcard(w => w
                    .Field(f => f.TitleStr).Value($"*{query.SearchString}*")
                    ) ||
                    q
                    .Wildcard(w => w
                    .Field(f => f.MarriageStatusStr).Value($"*{query.SearchString}*")
                    )
                    )

                    ).Size(50));
            return response.Result.Documents.Select(d => new PersonSearchResponse
            {
                Id = d.Id,
                FullName = HelperService.getCurrentLanguage().ToLower() == "am"
                    ? d.FirstNameAm + " " + d.MiddleNameAm + " " + d.LastNameAm
                    : d.FirstNameOr + " " + d.MiddleNameOr + " " + d.LastNameOr,
                Address = HelperService.getCurrentLanguage().ToLower() == "am"
                    ? d.AddressAm
                    : d.AddressOr,
                NationalId = d.NationalId,
                IsDead = d.DeathStatus,
                HasCivilMarriage = d.HasCivilMarriage

            }).ToList();
        }
        public async Task<List<PersonSearchResponse>> SearchSimilarPersons(SearchSimilarPersonalInfoQuery queryParams)
        {
            if (isAllNull(queryParams))
            {
                return new List<PersonSearchResponse>();
            }
            _elasticClient.Indices.Delete("personal_info");
            if (!_elasticClient.Indices.Exists("personal_info").Exists)
            {

                var indexRes = _elasticClient
                   .IndexMany<PersonalInfoIndex>(dbContext.PersonalInfos
                        .Where(p => p.DeathStatus == false)
                       .Select(p => new PersonalInfoIndex
                       {
                           Id = p.Id,
                           FirstNameStr = p.FirstNameStr,
                           FirstNameOr = p.FirstName == null ? null : p.FirstName.Value<string>("or"),
                           FirstNameAm = p.FirstName == null ? null : p.FirstName.Value<string>("am"),
                           MiddleNameStr = p.MiddleNameStr,
                           MiddleNameOr = p.MiddleName == null ? null : p.MiddleName.Value<string>("or"),
                           MiddleNameAm = p.MiddleName == null ? null : p.MiddleName.Value<string>("am"),
                           LastNameStr = p.LastNameStr,
                           LastNameOr = p.LastName == null ? null : p.LastName.Value<string>("or"),
                           LastNameAm = p.LastName == null ? null : p.LastName.Value<string>("am"),
                           NationalId = p.NationalId,
                           PhoneNumber = p.PhoneNumber,
                           BirthDate = p.BirthDate,
                           GenderOr = p.SexLookup.Value == null ? null : p.SexLookup.Value.Value<string>("or"),
                           GenderAm = p.SexLookup.Value == null ? null : p.SexLookup.Value.Value<string>("am"),
                           GenderStr = p.SexLookup.ValueStr,
                           TypeOfWorkStr = p.TypeOfWorkLookup.ValueStr,
                           TitleStr = p.TitleLookup.ValueStr,
                           MarriageStatusStr = p.MarraigeStatusLookup.ValueStr,
                           AddressOr = p.ResidentAddress.AddressName == null ? null : p.ResidentAddress.AddressName.Value<string>("or"),
                           AddressAm = p.ResidentAddress.AddressName == null ? null : p.ResidentAddress.AddressName.Value<string>("am"),
                           DeathStatus = p.DeathStatus
                       }), "personal_info");
                    await _elasticClient.Indices.RefreshAsync("personal_info");

            }


            var response = _elasticClient.SearchAsync<PersonalInfoIndex>(s => s
                    .Index("personal_info")
                    .Source(src => src
                    .Includes(i => i
                        .Fields(
                            f => f.Id,
                            f => f.FirstNameAm,
                            f => f.FirstNameOr,
                            f => f.MiddleNameAm,
                            f => f.MiddleNameOr,
                            f => f.LastNameAm,
                            f => f.LastNameOr,
                            f => f.AddressAm,
                            f => f.AddressOr,
                            f => f.NationalId

                        )
                    ))
                    .Query(q =>
                   q.Bool(b => b
                    .Must(
                        queryParams.FirstNameAm != null ?
                            mu => mu.Match(m => m
                                .Field(f => f.FirstNameAm)
                                .Query(queryParams.FirstNameAm)
                            ) : null,
                            mu => mu.Match(
                                queryParams.MiddleNameAm != null ?
                                m => m
                                .Field(f => f.MiddleNameAm)
                                .Query(queryParams.MiddleNameAm) : null
                            ),
                            mu => mu.Match(
                                queryParams.LastNameAm != null ?
                                m => m
                                .Field(f => f.LastNameAm)
                                .Query(queryParams.LastNameAm) : null
                            )
                        )
                        ) ||
                        q.Bool(b => b
                            .Must(
                                queryParams.FirstNameOr != null ?
                                    mu => mu.Match(m => m
                                        .Field(f => f.FirstNameOr)
                                        .Query(queryParams.FirstNameOr)
                                    ) : null,
                                    mu => mu.Match(
                                        queryParams.MiddleNameOr != null ?
                                        m => m
                                        .Field(f => f.MiddleNameOr)
                                        .Query(queryParams.MiddleNameOr) : null
                                    ),
                                    mu => mu.Match(
                                        queryParams.LastNameOr != null ?
                                        m => m
                                        .Field(f => f.LastNameOr)
                                        .Query(queryParams.LastNameOr) : null
                                    )
                        )
                        ) ||
                         q.Bool(b => b
                            .Must(
                                queryParams.PhoneNumber != null ?
                                    mu => mu.Match(m => m
                                        .Field(f => f.PhoneNumber)
                                        .Query(queryParams.PhoneNumber)
                                    ) : null

                        )
                        ) ||
                        q.Bool(b => b
                            .Must(
                                queryParams.NationalId != null ?
                                    mu => mu.Match(m => m
                                        .Field(f => f.NationalId)
                                        .Query(queryParams.NationalId)
                                    ) : null

                        )
                        )

                    )

                      .Size(50));

            return response.Result.Documents.Select(d => new PersonSearchResponse
            {
                Id = d.Id,
                FullName = HelperService.getCurrentLanguage().ToLower() == "am"
                    ? d.FirstNameAm + " " + d.MiddleNameAm + " " + d.LastNameAm
                    : d.FirstNameOr + " " + d.MiddleNameOr + " " + d.LastNameOr,
                Address = HelperService.getCurrentLanguage().ToLower() == "am"
                    ? d.AddressAm
                    : d.AddressOr,
                NationalId = d.NationalId

            }).ToList();


        }

        private bool isAllNull(SearchSimilarPersonalInfoQuery queryParams)
        {
            return queryParams.FirstNameAm == null && queryParams.FirstNameOr == null
            && queryParams.MiddleNameAm == null && queryParams.MiddleNameOr == null
            && queryParams.LastNameAm == null && queryParams.LastNameOr == null
            && queryParams.NationalId == null && queryParams.PhoneNumber == null;
        }

    }
}