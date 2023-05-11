using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Mapper;

namespace AppDiv.CRVS.Application.Features.Courts.Commmands.Create
{
    public class CreateCourtCommandHandler : IRequestHandler<CreateCourtCommand, CreateCourtCommandResponse>
    {
        private readonly ICourtRepository _courtRepository;
        private readonly ILogger<CreateCourtCommandHandler> _logger;
        public CreateCourtCommandHandler(ICourtRepository courtlookupRepository, ILogger<CreateCourtCommandHandler> logger)
        {
            _courtRepository = courtlookupRepository;
            _logger = logger;
        }
        public async Task<CreateCourtCommandResponse> Handle(CreateCourtCommand request, CancellationToken cancellationToken)
        {
            var CreatecourtCommadResponse = new CreateCourtCommandResponse();

            var validator = new CreateCourtCommandValidetor(_courtRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.Errors.Count > 0)
            {
                CreatecourtCommadResponse.Success = false;
                CreatecourtCommadResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreatecourtCommadResponse.ValidationErrors.Add(error.ErrorMessage);
                CreatecourtCommadResponse.Message = CreatecourtCommadResponse.ValidationErrors[0];
            }
            if (CreatecourtCommadResponse.Success)
            {
                var court = CustomMapper.Mapper.Map<Court>(request.court);
                await _courtRepository.InsertAsync(court, cancellationToken);
                var result = await _courtRepository.SaveChangesAsync(cancellationToken);

            }
            return CreatecourtCommadResponse;
        }
    }
}
