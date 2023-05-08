using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create
{

    public class CreateMarriageApplicationCommandHandler : IRequestHandler<CreateMarriageApplicationCommand, CreateMarriageApplicationCommandResponse>
    {
        private readonly IMarriageApplicationRepository _marriageApplicationRepository;

        public CreateMarriageApplicationCommandHandler(IMarriageApplicationRepository marriageApplicationRepository)
        {
            _marriageApplicationRepository = marriageApplicationRepository;
        }
        public async Task<CreateMarriageApplicationCommandResponse> Handle(CreateMarriageApplicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _marriageApplicationRepository.InsertAsync(
                    CustomMapper.Mapper.Map<MarriageApplication>(request), 
                    cancellationToken);
                await _marriageApplicationRepository.SaveChangesAsync(cancellationToken);

            }
            catch (System.Exception)
            {

                throw;
            }

            return new CreateMarriageApplicationCommandResponse{Message = "create marriage application successfull"};
        }
    }
}
