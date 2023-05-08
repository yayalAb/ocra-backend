
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Utility.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.MarriageApplications.Query
{
    // Customer query with List<Customer> response
    public record GetMarriageApplicationByIdQuery : IRequest<UpdateMarriageApplicationCommand>
    {
        public Guid Id { get; set; }
    }

    public class GetMarriageApplicationByIdQueryHandler : IRequestHandler<GetMarriageApplicationByIdQuery, UpdateMarriageApplicationCommand>
    {
        private readonly IMarriageApplicationRepository _marriageApplicationRepository;
        private readonly IMapper _mapper;

        public GetMarriageApplicationByIdQueryHandler(IMarriageApplicationRepository marriageApplicationRepository , IMapper mapper)
        {
            _marriageApplicationRepository = marriageApplicationRepository;
            _mapper = mapper;
        }
        public async Task<UpdateMarriageApplicationCommand> Handle(GetMarriageApplicationByIdQuery request, CancellationToken cancellationToken)
        {
           
        //      var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
        //                                         {
        //                                             { "BrideInfo", NavigationPropertyType.REFERENCE },
        //                                             { "GroomInfo", NavigationPropertyType.REFERENCE },
        //                                             { "GroomInfo.ContactInfo", NavigationPropertyType.REFERENCE },
        //                                             { "BrideInfo.ContactInfo", NavigationPropertyType.REFERENCE },



                                                  
        //                                         };
        //    return _mapper.Map<UpdateMarriageApplicationCommand>( 
        //         await _marriageApplicationRepository.GetWithAsync(request.Id,explicitLoadedProperties));
        var marriageApplication =  await _marriageApplicationRepository
                .GetAll().Where(m => m.Id == request.Id)
                .Include(m => m.BrideInfo)
                .ThenInclude(b => b.ContactInfo)
                .Include(m => m.GroomInfo).ThenInclude(g => g.ContactInfo)
                .ProjectTo<UpdateMarriageApplicationCommand>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            if(marriageApplication == null){
                throw new NotFoundException($"marriage application with id {request.Id} not found");
            }
            return marriageApplication;
        }
    }
}