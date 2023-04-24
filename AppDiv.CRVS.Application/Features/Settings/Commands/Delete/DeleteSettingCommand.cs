using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Settings.create
{
    // Customer create command with string response
    public class DeleteSettingCommand : IRequest<String>
    {
        public Guid Id { get;  set; }

    }

    // Customer delete command handler with string response as output
    public class DeleteSettingCommandHandler : IRequestHandler<DeleteSettingCommand, String>
    {
        private readonly ISettingRepository _settingRepository;
        public DeleteSettingCommandHandler(ILookupRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<string> Handle(DeleteSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var settingEntity = await _settingRepository.GetByIdAsync(request.Id);

                await _settingRepository.DeleteAsync(settingEntity);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Lookup information has been deleted!";
        }
    }
}