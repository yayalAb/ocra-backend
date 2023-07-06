
using System.Text.RegularExpressions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Domain.Repositories;

namespace AppDiv.CRVS.Application.Features.PaymentRequest.PaymentRequestQuery
{
    // Customer PaymentRequestListQuery with  response
    public class PaymentRequestListQuery : IRequest<PaginatedList<PaymentRequestListDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
    }

    public class GetListOfLookupQueryHandler : IRequestHandler<PaymentRequestListQuery, PaginatedList<PaymentRequestListDTO>>
    {
        private readonly IPaymentRequestRepository _PaymentRequestRepository;
        private readonly IUserResolverService _userResolverService;
        private readonly IUserRepository _userRepository;

        public GetListOfLookupQueryHandler(IPaymentRequestRepository PaymentRequestRepository, IUserRepository userRepository, IUserResolverService userResolverService)
        {
            _PaymentRequestRepository = PaymentRequestRepository;
            _userRepository = userRepository;
            _userResolverService = userResolverService;
        }
        public async Task<PaginatedList<PaymentRequestListDTO>> Handle(PaymentRequestListQuery request, CancellationToken cancellationToken)
        {
            var user = _userRepository.GetAll().Where(x => x.PersonalInfoId == _userResolverService.GetUserPersonalId()).FirstOrDefault();
            if (user == null)
            {
                throw NotFoundException("User Not Found");
            }
            var paymentRequestList = _PaymentRequestRepository.GetAll()
            .Include(x => x.Request)
            .Include(x => x.Request.CivilRegOfficer)
            .Include(x => x.Event)
            .Where(x => x.status == false && x.Event.EventRegisteredAddressId == user.AddressId);
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                paymentRequestList = paymentRequestList.Where(
                    u => EF.Functions.Like(u.ReasonStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Event.EventOwener.FirstNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Event.EventOwener.MiddleNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Event.EventOwener.LastNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Amount.ToString()!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.CreatedAt.ToString(), "%" + request.SearchString + "%"));
            }
            return
            await PaginatedList<PaymentRequestListDTO>
                           .CreateAsync(
                                paymentRequestList
                                    .Select(x => new PaymentRequestListDTO
                                    {
                                        Id = x.Id,
                                        RequestType = x.Reason.Value<string>("en"),
                                        Amount = x.Amount,
                                        RequestedBy = x.Event.EventOwener.FirstNameLang + " " + x.Event.EventOwener.MiddleNameLang + " " + x.Event.EventOwener.LastNameLang,
                                        RequestedDate = x.CreatedAt,
                                    })
                               , request.PageCount ?? 1, request.PageSize ?? 10);
        }

        private Exception NotFoundException(string v)
        {
            throw new NotImplementedException();
        }
    }
}

