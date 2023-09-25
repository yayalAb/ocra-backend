using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Ranges.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetRangeByIdQuery : IRequest<RangeDTO>
    {
        public Guid Id { get; set; }

        // public GetRangeByIdQuery(Guid Id)
        // {
        //     this.Id = Id;
        // }

    }

    public class GetRangeByIdHandler : IRequestHandler<GetRangeByIdQuery, RangeDTO>
    {
        private readonly IRangeRepository _rangeRepository;

        public GetRangeByIdHandler(IRangeRepository rangeRepository)
        {
            _rangeRepository = rangeRepository;
        }
        public async Task<RangeDTO> Handle(GetRangeByIdQuery request, CancellationToken cancellationToken)
        {
            
            var selectedRange = _rangeRepository.GetAll()
                .Where(p => p.Id == request.Id)
                .Select(p => new RangeDTO
                {
                    Id = p.Id,
                    Key = p.Key,
                    Start = p.Start,
                    End = p.End
                }).SingleOrDefault();
            return selectedRange;
            // return selectedCustomer;
        }
    }
}