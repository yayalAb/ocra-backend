using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookup.Command.Delete
{
    // Customer create command with string response
    public class DeleteLookupCommand : IRequest<String>
    {
        public Guid Id { get; private set; }

        public DeleteLookupCommand(Guid Id)
        {
            this.Id = Id;
        }
    }

    // Customer delete command handler with string response as output
    public class DeleteLookupCommandHandler : IRequestHandler<DeleteLookupCommand, String>
    {
        private readonly ILookupRepository _lookupRepository;
        public DeleteLookupCommandHandler(ILookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        public async Task<string> Handle(DeleteLookupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerEntity = await _lookupRepository.GetByIdAsync(request.Id);

                await _lookupRepository.DeleteAsync(customerEntity);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Lookup information has been deleted!";
        }
    }
}