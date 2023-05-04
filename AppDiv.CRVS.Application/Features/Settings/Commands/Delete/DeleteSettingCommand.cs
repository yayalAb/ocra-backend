using AppDiv.CRVS.Application.Common;
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
    // Customer create command with BaseResponse response
    public class DeleteSettingCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class DeleteSettingCommandHandler : IRequestHandler<DeleteSettingCommand, BaseResponse>
    {
        private readonly ISettingRepository _settingRepository;
        public DeleteSettingCommandHandler(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<BaseResponse> Handle(DeleteSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var settingEntity = await _settingRepository.GetByIdAsync(request.Id);

                await _settingRepository.DeleteAsync(request.Id);
                await _settingRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            var res = new BaseResponse
            {
                Success = true,
                Message = "Setting information has been deleted!"
            };


            return res;
        }
    }
}