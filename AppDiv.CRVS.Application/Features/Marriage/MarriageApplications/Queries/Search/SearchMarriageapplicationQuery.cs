using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Utility.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.Marriage.MarriageApplications.Queries.Search
{
    // Customer query with List<Customer> response
    public record SearchMarriageapplicationQuery : IRequest<List<MarriageApplicationSearchDto>>
    {
        public String SearchString { get; set; }
    }

    public class SearchMarriageapplicationQueryHandler : IRequestHandler<SearchMarriageapplicationQuery, List<MarriageApplicationSearchDto>>
    {
        private readonly IMarriageApplicationRepository _marriageApplicationRepository;
        private readonly IMapper _mapper;

        public SearchMarriageapplicationQueryHandler(IMarriageApplicationRepository marriageApplicationRepository, IMapper mapper)
        {
            _marriageApplicationRepository = marriageApplicationRepository;
            _mapper = mapper;
        }
        public async Task<List<MarriageApplicationSearchDto>> Handle(SearchMarriageapplicationQuery request, CancellationToken cancellationToken)
        {
            var marriageApplication = _marriageApplicationRepository.GetAll()
            .Include(model => model.MarriageEvent)
            .Where(model => model.MarriageEvent == null &&
                          (EF.Functions.Like(model.GroomInfo.FirstNameStr, $"%{request.SearchString}%")
                        || EF.Functions.Like(model.GroomInfo.LastNameStr, $"%{request.SearchString}%")
                        || EF.Functions.Like(model.GroomInfo.MiddleNameStr, $"%{request.SearchString}%")
                        || EF.Functions.Like(model.BrideInfo.FirstNameStr, $"%{request.SearchString}%")
                        || EF.Functions.Like(model.BrideInfo.LastNameStr, $"%{request.SearchString}%")
                        || EF.Functions.Like(model.BrideInfo.MiddleNameStr, $"%{request.SearchString}%")
                         || EF.Functions.Like(model.CivilRegOfficer.FirstNameStr, $"%{request.SearchString}%")
                        || EF.Functions.Like(model.CivilRegOfficer.LastNameStr, $"%{request.SearchString}%")
                        || EF.Functions.Like(model.CivilRegOfficer.MiddleNameStr, $"%{request.SearchString}%")
                        ))

                        .Select(x => new MarriageApplicationSearchDto
                        {
                            Id = x.Id,
                            GroomFullName = x.GroomInfo.FirstNameLang + " " + x.GroomInfo.MiddleNameLang + " " + x.GroomInfo.LastNameLang,
                            BridFullName = x.BrideInfo.FirstNameLang + " " + x.BrideInfo.MiddleNameLang + " " + x.BrideInfo.LastNameLang,
                            CicilRegOfficerFullName = x.CivilRegOfficer.FirstNameLang + " " + x.CivilRegOfficer.MiddleNameLang + " " + x.CivilRegOfficer.LastNameLang,
                            ApplicationDate = x.ApplicationDate
                        }).Take(50);


            // var marriageapp = CustomMapper.Mapper.Map<MarriageApplicationGridDTO>(marriageApplication);


            return marriageApplication.ToList();
        }
    }
}