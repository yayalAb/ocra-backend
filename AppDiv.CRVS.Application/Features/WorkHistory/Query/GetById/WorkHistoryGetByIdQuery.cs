using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using MediatR;

namespace AppDiv.CRVS.Application.Features.WorkHistorys.Query.GetById
{
    // Customer WorkHistoryGetByIdQuery with  response
    public class WorkHistoryGetByIdQuery : IRequest<List<WorkHistoryDTO>>
    {
        public string Id { get; private set; }

        public WorkHistoryGetByIdQuery(string Id)
        {
            this.Id = Id;
        }

    }

    public class WorkHistoryGetByIdQueryHandler : IRequestHandler<WorkHistoryGetByIdQuery, List<WorkHistoryDTO>>
    {

        private readonly IWorkHistoryRepository _workHistoryRepository;

        public WorkHistoryGetByIdQueryHandler(IWorkHistoryRepository workHistoryRepository)
        {
            _workHistoryRepository = workHistoryRepository;
        }
        public async Task<List<WorkHistoryDTO>> Handle(WorkHistoryGetByIdQuery request, CancellationToken cancellationToken)
        {
            return _workHistoryRepository.GetAll(request.Id).Select(w => new WorkHistoryDTO(w)).ToList();
        }
    }
}