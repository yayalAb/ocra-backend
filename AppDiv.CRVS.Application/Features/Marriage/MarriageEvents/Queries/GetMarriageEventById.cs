
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Utility.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Query
{
    // Customer query with List<Customer> response
    public record GetMarriageEventByIdQuery : IRequest<UpdateMarriageEventCommand>
    {
        public Guid Id { get; set; }
    }

    public class GetMarriageEventByIdQueryHandler : IRequestHandler<GetMarriageEventByIdQuery, UpdateMarriageEventCommand>
    {
        private readonly IMarriageEventRepository _MarriageEventRepository;
        private readonly IMapper _mapper;

        public GetMarriageEventByIdQueryHandler(IMarriageEventRepository MarriageEventRepository , IMapper mapper)
        {
            _MarriageEventRepository = MarriageEventRepository;
            _mapper = mapper;
        }
        public async Task<UpdateMarriageEventCommand> Handle(GetMarriageEventByIdQuery request, CancellationToken cancellationToken)
        {
    
        var MarriageEvent =  await _MarriageEventRepository
                .GetAll().Where(m => m.Id == request.Id)
                .Include(m => m.BrideInfo)
                .ThenInclude(b => b.ContactInfo)
                .Include(m => m.Event)
                .Include(m => m.Event.EventOwener).ThenInclude(e => e.ContactInfo)
                .Include(m => m.Event.EventSupportingDocuments)
                .Include(m=> m.Event.PaymentExamption).ThenInclude(p => p.SupportingDocuments)
                .ProjectTo<UpdateMarriageEventCommand>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            if(MarriageEvent == null){
                throw new NotFoundException($"marriage Event with id {request.Id} not found");
            }
            return MarriageEvent;
        }
    }
}