using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;

namespace AppDiv.CRVS.Application.Features.MarriageEvent.Command.Create
{

    public class CreateMarriageEventCommandHandler : IRequestHandler<CreateMarriageEventCommand, CreateMarriageEventCommandResponse>
    {
        public CreateMarriageEventCommandHandler()
        {

        }
        public async Task<CreateMarriageEventCommandResponse> Handle(CreateMarriageEventCommand request, CancellationToken cancellationToken)
        {

           
            return new CreateMarriageEventCommandResponse();
        }
    }
}
