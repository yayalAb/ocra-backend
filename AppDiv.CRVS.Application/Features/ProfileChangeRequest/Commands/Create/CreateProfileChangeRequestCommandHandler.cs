using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.ProfileChangeRequests.Commands.Create
{
    public class CreateProfileChangeRequestCommandHandler : IRequestHandler<CreateProfileChangeRequestCommand, BaseResponse>
    {
        private readonly IProfileChangeRequestRepository _ProfileChangeRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IWorkflowRepository _WorkflowRepository;
        private readonly ITransactionService _transactionService;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IContentValidator _contentValidator;


        public CreateProfileChangeRequestCommandHandler(IProfileChangeRequestRepository ProfileChangeRepository,
                                              IWorkflowService WorkflowService,
                                              IWorkflowRepository WorkflowRepository,
                                              ITransactionService transactionService,
                                              IUserRepository userRepository,
                                              INotificationService notificationService,
                                              IContentValidator contentValidator
                                              )
        {
            if (contentValidator is null)
            {
                throw new ArgumentNullException(nameof(contentValidator));
            }
            _ProfileChangeRepository = ProfileChangeRepository;
            _WorkflowService = WorkflowService;
            _WorkflowRepository = WorkflowRepository;
            _transactionService = transactionService;
            _userRepository = userRepository;
            _notificationService = notificationService;
            this._contentValidator = contentValidator;
        }

        public async Task<BaseResponse> Handle(CreateProfileChangeRequestCommand request, CancellationToken cancellationToken)
        {
            bool hasWorkflow = false;
            var executionStrategy = _ProfileChangeRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = _ProfileChangeRepository.Database.BeginTransaction())
                {
                    try

                    {
                        var response = new BaseResponse();
                        request.Request = new AddRequest
                        {
                            RequestType = "profile change",
                            currentStep = 0,
                        };

                        var profileChangeRequest = CustomMapper.Mapper.Map<ProfileChangeRequest>(request);
                        var userData = await _userRepository.GetAll().Include(u => u.UserGroups).Where(e => e.Id == request.UserId).FirstOrDefaultAsync();
                        if (userData == null)
                        {
                            throw new NotFoundException($"user with id {request.UserId} is not found");
                        }
                        var Workflow = _WorkflowRepository.GetAll()
                        .Include(x => x.Steps)
                           .Where(wf => wf.workflowName == "profile change").FirstOrDefault();
                        if ((Workflow?.Id != null && Workflow?.Id != Guid.Empty) && (Workflow?.Steps?.FirstOrDefault() != null))
                        {
                            hasWorkflow = true;
                            profileChangeRequest.Request.WorkflowId = Workflow.Id;
                            profileChangeRequest.Request.NextStep = _WorkflowService.GetNextStep("profile change", 0, true);
                        }
                        var validationResponse = await _contentValidator.ValidateUserDataAsync(userData, profileChangeRequest.Content, !hasWorkflow);
                        if (!hasWorkflow)
                        {
                            await transaction.CommitAsync();
                            return validationResponse;
                        }
                        else
                        {
                            if (validationResponse.Status != 200)
                            {
                                return validationResponse;
                            }

                            profileChangeRequest.Request.CivilRegOfficerId = userData.PersonalInfoId;
                            await _ProfileChangeRepository.InsertAsync(profileChangeRequest, cancellationToken);
                            var result = await _ProfileChangeRepository.SaveChangesAsync(cancellationToken);
                            var NewTranscation = new TransactionRequestDTO
                            {
                                CurrentStep = 0,
                                ApprovalStatus = true,
                                WorkflowId = Workflow.Id,
                                RequestId = profileChangeRequest.RequestId,
                                CivilRegOfficerId = userData.Id,//_UserResolverService.GetUserId().ToString(),
                                Remark = request.Remark
                            };

                            await _transactionService.CreateTransaction(NewTranscation);
                            await _notificationService.CreateNotification(profileChangeRequest.Id, Enum.GetName<NotificationType>(NotificationType.profileChange)!,request.Remark,
                                               _WorkflowService.GetReceiverGroupId(Enum.GetName<NotificationType>(NotificationType.change)!, (int)profileChangeRequest.Request.NextStep), profileChangeRequest.RequestId,
                                             userData.Id, userData.AddressId, "request", null);
                            await transaction.CommitAsync();
                            response.Message = "profileChangeRequest created successfully";
                            return response;
                        }
                    }
                    catch (Exception)
                    {

                        await transaction.RollbackAsync();
                        throw;
                    }
                }

            });
        }

    }
}
