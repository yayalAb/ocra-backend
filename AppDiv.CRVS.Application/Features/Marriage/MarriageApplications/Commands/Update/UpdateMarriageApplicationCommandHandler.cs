using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update
{

    public class UpdateMarriageApplicationCommandHandler : IRequestHandler<UpdateMarriageApplicationCommand, UpdateMarriageApplicationCommandResponse>
    {
        private readonly IMarriageApplicationRepository _marriageApplicationRepository;

        public UpdateMarriageApplicationCommandHandler(IMarriageApplicationRepository marriageApplicationRepository)
        {
            _marriageApplicationRepository = marriageApplicationRepository;
        }
        public async Task<UpdateMarriageApplicationCommandResponse> Handle(UpdateMarriageApplicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
            //    var marriageApplication = _marriageApplicationRepository.GetAsync(request.Id);
               if(!_marriageApplicationRepository.exists(request.Id)){
                throw new NotFoundException($"marriage application with {request.Id} is not found");
               }
                 _marriageApplicationRepository.EFUpdate(CustomMapper.Mapper.Map<MarriageApplication>(request));
                await _marriageApplicationRepository.SaveChangesAsync(cancellationToken);

            }
            catch (System.Exception)
            {

                throw;
            }

            return new UpdateMarriageApplicationCommandResponse{Message = "Update marriage application successfull"};
        }
    }
}
