using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Features.Search;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Nest;

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
        // public async Task<PersonalInfoSearchDTO> SearchPersonalInfo(GetPersonalInfoQuery query){

        //     var response = _elasticClient.SearchAsync<PersonalInfoIndex>(s => s
        //             .Index("personal_info").Query());
        // }
        public async Task<List<PersonalInfoIndex>> SearchElastic(SearchSimilarPersonalInfoQuery queryParams)
        {
            // _elasticClient.Indices.Delete("personal_info");
            if (!_elasticClient.Indices.Exists("personal_info").Exists)
            {

                var indexRes = _elasticClient
                   .IndexMany<PersonalInfoIndex>(dbContext.PersonalInfos
                       .Select(p => new PersonalInfoIndex
                       {
                           Id = p.Id,
                           FirstNameOr = p.FirstName.Value<string>("or"),
                           FirstNameAm = p.FirstName.Value<string>("am"),
                           MiddleNameOr = p.MiddleName.Value<string>("or"),
                           MiddleNameAm = p.MiddleName.Value<string>("am"),
                           LastNameOr = p.LastName.Value<string>("or"),
                           LastNameAm  = p.LastName.Value<string>("am"),
                           NationalId = p.NationalId,
                           PhoneNumber = p.PhoneNumber,
                           GenderOr = p.SexLookup.Value.Value<string>("or"),
                           GenderAm = p.SexLookup.Value.Value<string>("am"),
                           BirthDate = p.BirthDate

                           
                       }).ToList(), "personal_info");
                var second = "kjdkj";
                // _elasticClient.Indices.Create("person2", index => index.Map<PersonDtoElastic>(x => x.AutoMap()));
            }

            var response = _elasticClient.SearchAsync<PersonalInfoIndex>(s => s
                    .Index("personal_info")
                    .Query(q =>
                    // q.Bool(b => b
                    //     .Must(m => m.MatchAll())
                    //     .Should(
                    //         queryParams.FirstName != null ?
                    //             new QueryContainer[] { new MatchQuery { Field = "FirstNameStr", Query = queryParams.FirstName.ToString() } }
                    //             : null
                    //         )
                    // ) &&
                    // q.Bool(b => b
                    //     .Must(m => m.MatchAll())
                    //     .Should(
                    //         queryParams.MiddleName != null ?
                    //             new QueryContainer[] { new MatchQuery { Field = "MiddleNameStr", Query = queryParams.MiddleName.ToString() } }
                    //             : null
                    //         )
                    // ) &&
                    // q.Bool(b => b
                    //     .Must(m => m.MatchAll())
                    //     .Should(
                    //         queryParams.LastName != null ?
                    //             new QueryContainer[] { new MatchQuery { Field = "LastNameStr", Query = queryParams.LastName.ToString() } }
                    //             : null
                    //         )
                    // ) &&
                    // q.Bool(b => b
                    //     .Must(m => m.MatchAll())
                    //     .Should(
                    //         queryParams.PhoneNumber != null ?
                    //             new QueryContainer[] { new MatchQuery { Field = "PhoneNumber", Query = queryParams.PhoneNumber } }
                    //             : null
                    //         )
                    // ) 
                    // &&
                    // q.Term(t => t.NationalId, queryParams.NationalId) 
                    // q.MultiMatch()
                    // q.Bool(b => b
                    //     // .Must(m => m.MatchAll())
                    //     .Must(
                    //           queryParams.NationalId != null ?
                    //        //    sh => sh.Match(m => m.Field(f => f.NationalId).Query(queryParams.NationalId)) : null,
                    //        sh => sh.Term(t => t.NationalId, queryParams.NationalId) : null,
                    //        queryParams.NationalId != null ?
                    //        sh => sh.Term(t => t.PhoneNumber, queryParams.PhoneNumber) : null

                    //         //    queryParams.NationalId != null ?
                    //         //    sh => sh.Match(m => m.Field(f => f.PhoneNumber).Query(queryParams.PhoneNumber)):null

                    //         )
                    // )
                   q.Bool(b => b
                   .Must(m => m.MatchAll())
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
                                queryParams.FirstNameOr!= null ?
                                    mu => mu.Match(m => m
                                        .Field(f => f.FirstNameOr)
                                        .Query(queryParams.FirstNameOr)
                                    ) : null,
                                    mu => mu.Match(
                                        queryParams.MiddleNameOr!= null ?
                                        m => m
                                        .Field(f => f.MiddleNameOr)
                                        .Query(queryParams.MiddleNameOr) : null
                                    ),
                                    mu => mu.Match(
                                        queryParams.LastNameOr!= null ?
                                        m => m
                                        .Field(f => f.LastNameOr)
                                        .Query(queryParams.LastNameOr) : null
                                    )
                        )
                        )||
                         q.Bool(b => b
                            .Must(
                                queryParams.PhoneNumber!= null ?
                                    mu => mu.Match(m => m
                                        .Field(f => f.PhoneNumber)
                                        .Query(queryParams.PhoneNumber)
                                    ) : null
                                    
                        )
                        )||
                        q.Bool(b => b
                            .Must(
                                queryParams.NationalId!= null ?
                                    mu => mu.Match(m => m
                                        .Field(f => f.NationalId)
                                        .Query(queryParams.NationalId)
                                    ) : null
                                    
                        )
                        )
                    // q.Bool(b => b
                    //     .Must(m => m.MatchAll())
                    //     .Should(
                    //         queryParams.NationalId != null ?
                    //             new QueryContainer[] { new MatchQuery { Field = "NationalId", Query = queryParams.NationalId } }
                    //             : null
                    //         )
                    // )
                    )
                      .Size(50));
            // var response = _elasticClient.Search<PersonalInfoSearchDTO>(s => s
            //                     .Query(q => q
            //                         .QueryString(qs => qs
            //                             .Query($"(NationalId:{queryParams.NationalId} AND PhoneNumber:{queryParams.PhoneNumber}) OR NationalId:string")
            //                         )
            //                     )
            //                 );
            var result = response.Result.Documents.ToList();
            return result;


        }


    }
}