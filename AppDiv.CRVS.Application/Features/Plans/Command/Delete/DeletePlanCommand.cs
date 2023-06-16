using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Plans.Command.Delete
{
    // Customer create command with string response
    public class DeletePlanCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; private set; }

        public DeletePlanCommand(Guid Id)
        {
            this.Id = Id;
        }
    }

    // Customer delete command handler with string response as output
    public class DeletePlanCommmandHandler : IRequestHandler<DeletePlanCommand, BaseResponse>
    {
        private readonly IPlanRepository _planRepository;
        public DeletePlanCommmandHandler(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<BaseResponse> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                var planEntity = await _planRepository.GetAsync(request.Id);
                if (planEntity != null)
                {
                    await _planRepository.DeleteAsync(request.Id);
                    await _planRepository.SaveChangesAsync(cancellationToken);
                    response.Deleted("Payment rate");
                }
                else
                {
                    response.BadRequest("There is no payment rate with the specified id");
                }

            }
            catch (Exception exp)
            {
                response.BadRequest("Unable to delete the payment rate.");
            }
            return response;
        }
    }
}