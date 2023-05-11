
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Courts.Query.GetById
{
    // Customer CourtGetByIdQuery with  response
    public class CourtGetByIdQuery : IRequest<CourtDTO>
    {
        public Guid Id { get; private set; }

        public CourtGetByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class CourtGetByIdQueryHandler : IRequestHandler<CourtGetByIdQuery, CourtDTO>
    {

        private readonly ICourtRepository _courtRepository;

        public CourtGetByIdQueryHandler(ICourtRepository courtRepository)
        {
            _courtRepository = courtRepository;
        }
        public async Task<CourtDTO> Handle(CourtGetByIdQuery request, CancellationToken cancellationToken)
        {
            // var lookups = await _mediator.Send(new GetAllLookupQuery());
            var selectedlookup = await _courtRepository.GetAsync(request.Id);
            return CustomMapper.Mapper.Map<CourtDTO>(selectedlookup);
            // return selectedCustomer;
        }
    }
}