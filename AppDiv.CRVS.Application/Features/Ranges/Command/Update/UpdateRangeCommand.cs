using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using AppDiv.CRVS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Ranges.Command.Update
{
    // Customer create command with CustomerResponse
    public class UpdateRangeCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

    }

    public class UpdateRangeCommandHandler : IRequestHandler<UpdateRangeCommand, BaseResponse>
    {
        private readonly IRangeRepository _rangeRepository;
        public UpdateRangeCommandHandler(IRangeRepository rangeRepository)
        {
            _rangeRepository = rangeRepository;
        }
        public async Task<BaseResponse> Handle(UpdateRangeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                var range = CustomMapper.Mapper.Map<SystemRange>(request);
                _rangeRepository.Update(range);
                var result = await _rangeRepository.SaveChangesAsync(cancellationToken);
                response.Status = 200;
                response.Message = "Range Updated Succesfully";
            }
            catch (Exception exp)
            {
                response.Status = 400;
                response.Success = false;
                response.Message = "Unable to update the range";
                response.ValidationErrors = new List<string> { exp.Message };
                return response;
                // throw new ApplicationException(exp.Message);
            }

            return response;
        }
    }
}