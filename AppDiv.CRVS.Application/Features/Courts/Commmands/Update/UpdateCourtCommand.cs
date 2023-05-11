using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Courts.Commmands.Update
{
    // Customer create command with CustomerResponse
    public class UpdateCourtCommand : IRequest<CourtDTO>
    {

        public Guid Id { get; set; }
        public virtual AddAddressRequest Address { get; set; }
        public JObject Name { get; set; }
        public JObject Description { get; set; }
    }

    public class UpdateCourtCommandHandler : IRequestHandler<UpdateCourtCommand, CourtDTO>
    {
        private readonly ICourtRepository _courtRepository;
        public UpdateCourtCommandHandler(ICourtRepository courtRepository)
        {
            _courtRepository = courtRepository;
        }
        public async Task<CourtDTO> Handle(UpdateCourtCommand request, CancellationToken cancellationToken)
        {
            Court CourtEntity = new Court
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
            };
            try
            {
                await _courtRepository.UpdateAsync(CourtEntity, x => x.Id);
                await _courtRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw;
            }
            // var modifiedLookup = await _courtRepository.GetByIdAsync(request.Id);
            var CourtResponse = CustomMapper.Mapper.Map<CourtDTO>(CourtEntity);
            return CourtResponse;
        }
    }
}