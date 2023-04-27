using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Command.Update
{
    // Customer create command with CustomerResponse
    public class GroupupdateCommands : IRequest<GroupDTO>
    {

        public Guid id { get; set; }
        public string GroupName { get; set; }
        public JObject Description { get; set; }
        public JArray Roles { get; set; }
    }

    public class GroupupdateCommandsHandler : IRequestHandler<GroupupdateCommands, GroupDTO>
    {
        private readonly IGroupRepository _groupRepository;
        public GroupupdateCommandsHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<GroupDTO> Handle(GroupupdateCommands request, CancellationToken cancellationToken)
        {
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request);
            UserGroup groupEntity = new UserGroup
            {
                Id = request.id,
                GroupName = request.GroupName,
                Description = request.Description,
                Roles = request.Roles,
            };
            try
            {
                await _groupRepository.UpdateAsync(groupEntity, x => x.Id);
                await _groupRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var modifiedLookup = await _groupRepository.GetByIdAsync(request.id);
            var LookupResponse = CustomMapper.Mapper.Map<GroupDTO>(modifiedLookup);
            return LookupResponse;
        }
    }
}