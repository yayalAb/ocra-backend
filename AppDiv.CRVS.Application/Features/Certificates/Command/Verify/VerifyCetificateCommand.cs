using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;


namespace AppDiv.CRVS.Application.Features.Certificates.Command.Verify
{

    public class VerifyCertificateCommand : IRequest<BaseResponse>
    {
        public Guid EventId { get; set; }
    }

    public class VerifyCertificateCommandHandler : IRequestHandler<VerifyCertificateCommand, BaseResponse>
    {
        private readonly IEventRepository _eventRepository;
        public VerifyCertificateCommandHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task<BaseResponse> Handle(VerifyCertificateCommand request, CancellationToken cancellationToken)
        {
            var eventObj = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventObj == null)
            {
                throw new NotFoundException($"event with id = {request.EventId} is not found");
            }
            eventObj.IsVerified = true;
            await _eventRepository.UpdateAsync(eventObj, e => e.Id);
            await _eventRepository.SaveChangesAsync(cancellationToken);

            return new BaseResponse("event verified succesfully",true,200);

        }
    }
}