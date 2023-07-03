
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Utility.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.DivorceEvents.Query
{
    // Customer query with List<Customer> response
    public record GetWivesQuery : IRequest<List<PersonSearchResponse>>
    {
        public Guid HusbandId { get; set; }
    }

    public class GetWivesQueryHandler : IRequestHandler<GetWivesQuery, List<PersonSearchResponse>>
    {
        private readonly IDivorceEventRepository _DivorceEventRepository;
        private readonly IMapper _mapper;
        private readonly IPersonalInfoRepository _personalInfoRepository;

        public GetWivesQueryHandler(IDivorceEventRepository DivorceEventRepository,
                                    IMapper mapper,
                                    IPersonalInfoRepository personalInfoRepository)
        {
            _DivorceEventRepository = DivorceEventRepository;
            _mapper = mapper;
            _personalInfoRepository = personalInfoRepository;
        }
        public async Task<List<PersonSearchResponse>> Handle(GetWivesQuery request, CancellationToken cancellationToken)
        {
            var husbandInfo = await _personalInfoRepository.GetAll()
                    .Where(p => p.Id == request.HusbandId)
                    .Include(p => p.Events)
                        .ThenInclude(e => e.MarriageEvent.MarriageType)
                    .Include(p => p.Events)
                        .ThenInclude(e => e.MarriageEvent.BrideInfo.ResidentAddress)
                    .Include(p => p.Events)
                        .ThenInclude(e => e.MarriageEvent.BrideInfo.Events)
                        // .ThenInclude(b =>b.ResidentAddress)
                        .FirstOrDefaultAsync();
                // var vvv = husbandInfo?.Events.Where(e =>  e.EventType.ToLower() =="marriage"  
                //                 &&  (e.MarriageEvent.MarriageType.ValueStr.Contains("Seera Siivilii")|| e.MarriageEvent.MarriageType.ValueStr.Contains("በመዘጋጃ የተመዘገቡ"))
                //                 ).Any()  ;
                
            if(husbandInfo == null){
                throw new NotFoundException($"person with id {request.HusbandId} is not found");
            }
            return husbandInfo.Events.Where(e => e.MarriageEvent != null)
                            .Select(e => new PersonSearchResponse{
                                Id = e.MarriageEvent.BrideInfoId,
                                FullName = e.MarriageEvent.BrideInfo.FirstNameLang + " "+e.MarriageEvent.BrideInfo.MiddleNameLang+" "+e.MarriageEvent.BrideInfo.LastNameLang,
                                Address = e.MarriageEvent.BrideInfo.ResidentAddress.AddressNameLang,
                                NationalId = e.MarriageEvent.BrideInfo.NationalId,
                                IsDead = e.MarriageEvent.BrideInfo.DeathStatus,
                                // HasCivilMarriage = e.MarriageEvent.BrideInfo.Events.Where(e =>  e.EventType.ToLower() =="marriage"  
                                // &&  (EF.Functions.Like( e.MarriageEvent.MarriageType.ValueStr, "%Seera Siivilii%")|| EF.Functions.Like( e.MarriageEvent.MarriageType.ValueStr, "%በመዘጋጃ የተመዘገቡ%"))
                                // ).Any()  
                            }).ToList();
        }
    }
}