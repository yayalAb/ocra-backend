using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create
{

    public class CreateMarriageApplicationCommandHandler : IRequestHandler<CreateMarriageApplicationCommand, CreateMarriageApplicationCommandResponse>
    {
        private readonly IMarriageApplicationRepository _marriageApplicationRepository;
        private readonly ILookupRepository _lookupRepo;

        public CreateMarriageApplicationCommandHandler(IMarriageApplicationRepository marriageApplicationRepository, ILookupRepository lookupRepo)
        {
            _marriageApplicationRepository = marriageApplicationRepository;
            _lookupRepo = lookupRepo;
        }
        public async Task<CreateMarriageApplicationCommandResponse> Handle(CreateMarriageApplicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var marriageApplication = CustomMapper.Mapper.Map<MarriageApplication>(request);
                marriageApplication.BrideInfo.SexLookupId = _lookupRepo.GetAll().Where(l => l.Key == "sex")
                                                                    .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                                                                        || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                                                                        || EF.Functions.Like(l.ValueStr, "%Male%"))
                                                                    .Select(l => l.Id).FirstOrDefault();

                marriageApplication.GroomInfo.SexLookupId = _lookupRepo.GetAll().Where(l => l.Key == "sex")
                                                                        .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                                                                            || EF.Functions.Like(l.ValueStr, "%Dubara%")
                                                                            || EF.Functions.Like(l.ValueStr, "%Female%"))
                                                                        .Select(l => l.Id).FirstOrDefault();
                await _marriageApplicationRepository.InsertAsync(
                    marriageApplication,
                    cancellationToken);
                await _marriageApplicationRepository.SaveChangesAsync(cancellationToken);

            }
            catch (System.Exception)
            {

                throw;
            }

            return new CreateMarriageApplicationCommandResponse { Message = "create marriage application successfull" };
        }
    }
}
