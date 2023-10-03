using MediatR;

namespace AppDiv.CRVS.Application.Features.Authentication.Querys
{
    public class GetCount : IRequest<int>
    {
        public string RequestType { get; set; } = "change";
        public string Status { get; set; }
        public bool IsYourRequestList { get; set; } = false;
    }
    public class GetCountHandler : IRequestHandler<GetCount, int>
    {
        
        private readonly IMediator _mediator;

        public GetCountHandler(IMediator mediator)
        {
            this._mediator = mediator;

        }
        public async Task<int> Handle(GetCount request, CancellationToken cancellationToken)
        {
            var list = await _mediator.Send(new GetRequestByType 
                { 
                    RequestType = request.RequestType, 
                    Status = request.Status, 
                    IsYourRequestList = request.IsYourRequestList, 
                    PageCount = 1, 
                    PageSize = 1 
                });
            return list.TotalCount;
        }
    }
}
