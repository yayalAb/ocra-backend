
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.WorkFlows.Query.GetAllWorkFlow

{
    // Customer query with List<Customer> response
    public record GetAllWorkFlowQuery : IRequest<List<GetAllWorkFlowDTO>>
    {

    }

    public class GetAllWorkFlowQueryHandler : IRequestHandler<GetAllWorkFlowQuery, List<GetAllWorkFlowDTO>>
    {
        private readonly IWorkflowRepository _workflowRepository;

        public GetAllWorkFlowQueryHandler(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<List<GetAllWorkFlowDTO>> Handle(GetAllWorkFlowQuery request, CancellationToken cancellationToken)
        {
            var LookupList = await _workflowRepository.GetAllWithAsync("Steps");

            // var LookupList = await _workflowRepository.GetAllAsync();
            var workflow = LookupList.Select(wf => new GetAllWorkFlowDTO
            {
                id = wf.Id,
                workflowName = wf.workflowName,
            });
            var lookups = CustomMapper.Mapper.Map<List<GetAllWorkFlowDTO>>(LookupList);
            return workflow.ToList();

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }
    }
}

//  public class GetAllGetAllWorkFlowDTO
//     {
//         public Guid id { get; set; }
//         public string workflowName { get; set; }
//         public int step { get; set; }
//         public string responsibleGroup { get; set; }
//         public string payment { get; set; }
//         public Boolean status { get; set; }

//     }