
using System;
using MediatR;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Utility.Contracts;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Notification.Queries.GetNotificationByTransactionId

{
    public class GetNotificationByTransactionIdQuery : IRequest<NotificationData>
    {
        public Guid Id { get; set; }
    }

    public class GetNotificationByTransactionIdQueryHandler : IRequestHandler<GetNotificationByTransactionIdQuery, NotificationData>
    {
        private readonly ITransactionService _transactionService;

        public GetNotificationByTransactionIdQueryHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<NotificationData> Handle(GetNotificationByTransactionIdQuery request, CancellationToken cancellationToken)
        {
           var transaction = _transactionService.GetTransaction(request.Id);

            return new NotificationData
            {
                Message = transaction.Remark,
                ApprovalType = transaction.Request.RequestType,
                SenderId = transaction.Request.CivilRegOfficer.ApplicationUser.Id,
                SenderUserName = transaction.Request.CivilRegOfficer.ApplicationUser.UserName,
                SenderFullName = transaction.Request.CivilRegOfficer.FullNameLang,
                Date = (new CustomDateConverter(transaction.CreatedAt)).ethiopianDate
            };
        }
    }
}