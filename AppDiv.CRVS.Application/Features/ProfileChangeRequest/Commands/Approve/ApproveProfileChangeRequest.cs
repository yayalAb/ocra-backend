using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.ProfileChangeRequests.Commands.Approve;
public class ApproveProfileChangeRequestCommand : IRequest<BaseResponse>
{
    public Guid RequestId { get; set; }
    public string? Remark { get; set; }
    public JArray? RejectionReasons { get; set; }
    public bool IsApprove { get; set; }
    public Guid? ReasonLookupId { get; set; }

}
public class ApproveProfileChangeRequestCommandHandler : IRequestHandler<ApproveProfileChangeRequestCommand, BaseResponse>
{
    private readonly IProfileChangeRequestRepository _profileChangeRequestRepository;
    private readonly IContentValidator _contentValidator;
    private readonly IUserRepository _userRepository;
    private readonly IWorkflowService _workflowService;

    public ApproveProfileChangeRequestCommandHandler(IProfileChangeRequestRepository profileChangeRequestRepository,
                                                    IContentValidator contentValidator,
                                                    IUserRepository userRepository,
                                                     IWorkflowService workflowService)
    {
        _profileChangeRequestRepository = profileChangeRequestRepository;
        _contentValidator = contentValidator;
        _userRepository = userRepository;
        _workflowService = workflowService;
    }
    public async Task<BaseResponse> Handle(ApproveProfileChangeRequestCommand request, CancellationToken cancellationToken)
    {
        var executionStrategy = _profileChangeRequestRepository.Database.CreateExecutionStrategy();
        return await executionStrategy.ExecuteAsync(async () =>
        {

            using (var transaction = _profileChangeRequestRepository.Database.BeginTransaction())
            {
                try

                {
                    var Response = new BaseResponse();
                    var response = await _workflowService.ApproveService(request.RequestId, "profile change", request.IsApprove, request.Remark, request.RejectionReasons, request.ReasonLookupId, false, cancellationToken);
                    if (response.Item1)
                    {
                        var profileChangeRequest = _profileChangeRequestRepository.GetAll()
                       .AsNoTracking()
                       .Where(x => x.RequestId == request.RequestId)
                       .Include(x => x.Request).FirstOrDefault();
                        if (profileChangeRequest == null)
                        {
                            throw new NotFoundException($"profileChangeRequest with RequestId = {request.RequestId} is not found");

                        }
                        var oldUserData = _userRepository.GetAll()
                                        .AsNoTracking()
                                        .Include(u => u.UserGroups)
                                        .Where(u => u.Id == profileChangeRequest.UserId)
                                        .FirstOrDefault();
                        if (oldUserData == null)
                        {
                            throw new NotFoundException("old user data to be updated is not found");
                        }
                        var res = await _contentValidator.ValidateUserDataAsync(oldUserData, profileChangeRequest.Content, true);
                        Response = res;
                        // await _eventRepostory.SaveChangesAsync(cancellationToken);
                    }
                    else
                    {
                        Response = new BaseResponse
                        {
                            Success = true,
                            Message = request.IsApprove ? "Correction Request Approved Successfuly" : "Correction Request Rejected Successfuly",
                        };
                    }
                    await transaction.CommitAsync();
                    _profileChangeRequestRepository.TriggerPersonalInfoIndex();
                    return Response;
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