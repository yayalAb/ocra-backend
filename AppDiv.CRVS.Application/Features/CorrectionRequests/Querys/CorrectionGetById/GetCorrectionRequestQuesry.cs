using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Querys.CorrectionGetById
{
    // Customer GetCorrectionRequestQuesry with  response
    public class GetCorrectionRequestQuesry : IRequest<AddCorrectionRequest>
    {
        public Guid Id { get; private set; }

        public GetCorrectionRequestQuesry(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetCorrectionRequestQuesryHandler : IRequestHandler<GetCorrectionRequestQuesry, AddCorrectionRequest>
    {

        private readonly ICorrectionRequestRepostory _correctionRequestRepository;

        public GetCorrectionRequestQuesryHandler(ICorrectionRequestRepostory correctionRequestRepository)
        {
            _correctionRequestRepository = correctionRequestRepository;
        }
        public async Task<AddCorrectionRequest> Handle(GetCorrectionRequestQuesry request, CancellationToken cancellationToken)
        {
            var CorrectionRequest = _correctionRequestRepository.GetAll()
            .Include(x => x.Request).Where(x => x.Id == request.Id).FirstOrDefault();
            return CustomMapper.Mapper.Map<AddCorrectionRequest>(CorrectionRequest);
        }
    }
}