
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.Search
{
    // Customer SearchUserDropDownQuery with  response
    public class SearchUserDropDownQuery : IRequest<object>
    {
        public string SearchString { get; set; }


    }

    public class SearchUserDropDownQueryHandler : IRequestHandler<SearchUserDropDownQuery, object>
    {
        private readonly IUserRepository _userRepository;

        public SearchUserDropDownQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<object> Handle(SearchUserDropDownQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.GetAll()
                                            .Include(model => model.Address)
                                            .Where(model =>
                                                   EF.Functions.Like(model.Address.AddressNameStr, $"%{request.SearchString}%")
                                                 || EF.Functions.Like(model.UserName, $"%{request.SearchString}%")
                                                  || EF.Functions.Like(model.Email, $"%{request.SearchString}%")
                                                   || EF.Functions.Like(model.PersonalInfo.FirstNameStr, $"%{request.SearchString}%")
                                                    || EF.Functions.Like(model.PersonalInfo.MiddleNameStr, $"%{request.SearchString}%")
                                                     || EF.Functions.Like(model.PersonalInfo.LastNameStr, $"%{request.SearchString}%")
                                                     || EF.Functions.Like(model.PersonalInfo.PhoneNumber, $"%{request.SearchString}%")
                                                     || EF.Functions.Like(model.PhoneNumber, $"%{request.SearchString}%")

                                                ).Select(
                                                    model => new
                                                    {
                                                        Id = model.Id,
                                                        UserName = model.UserName,
                                                        Email = model.Email,
                                                        Address = model.Address.AddressNameLang,
                                                        FullName = model.PersonalInfo.FirstNameLang + " " +
                                                                    model.PersonalInfo.MiddleNameLang + " " +
                                                                    model.PersonalInfo.LastNameLang,
                                                        PhoneNumber = model.PhoneNumber
                                                    }
                                                ).Take(50).ToListAsync();

        }
    }
}