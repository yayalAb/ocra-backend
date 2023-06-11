
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;

using AppDiv.CRVS.Application.Exceptions;

namespace AppDiv.CRVS.Application.Features.Search
{
    // Customer GetParentInfoByPersonIdQuery with  response
    public class GetParentInfoByPersonIdQuery : IRequest<object>
    {
        public Guid Id { get; set; }

    }

    public class GetParentInfoByPersonIdQueryHandler : IRequestHandler<GetParentInfoByPersonIdQuery, object>
    {
        private readonly IPersonalInfoRepository _PersonaInfoRepository;

        public GetParentInfoByPersonIdQueryHandler(IPersonalInfoRepository PersonaInfoRepository)
        {
            _PersonaInfoRepository = PersonaInfoRepository;
        }
        public async Task<object> Handle(GetParentInfoByPersonIdQuery request, CancellationToken cancellationToken)
        {
            Guid? MotherId =null;
            Guid? FatherId =null;
            string From = "";
            var personEvents = await _PersonaInfoRepository.GetAll()
                     .Include(p => p.Events)
                     .Where(p => p.Id == request.Id)
                     .Select(p => new
                     {
                         adoption = p.Events.Where(e => e.EventType.ToLower() == "adoption" && e.EventOwenerId == request.Id)
                                        .Select(e => e.AdoptionEvent).FirstOrDefault(),
                         birth = p.Events.Where(e => e.EventType.ToLower() == "birth" && e.EventOwenerId == request.Id)
                                        .Select(e => e.BirthEvent).FirstOrDefault()
                     })
                     .FirstOrDefaultAsync();

            if (personEvents == null)
            {
                throw new NotFoundException($"personalinfo with id {request.Id} is not found");
            }
            if (personEvents?.adoption != null)
            {
                MotherId = personEvents.adoption.AdoptiveMotherId;
                FatherId = personEvents.adoption.AdoptiveFatherId;
                From = "Adoption Certificate";
            }
            else if (personEvents?.birth != null)
            {
                MotherId = personEvents.birth.MotherId;
                FatherId = personEvents.birth.FatherId;
                From = "Birth Certificate";
            }
            return new { MotherId = MotherId, FatherId = FatherId, From = From };
        }
    }
}