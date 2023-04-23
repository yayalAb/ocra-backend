using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Delete
{
    // Customer create command with string response
    public class DeleteAddressCommand : IRequest<String>
    {
        public Guid Id { get; set; }

    }

    // Customer delete command handler with string response as output
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, String>
    {
        private readonly IAddressLookupRepository _addressRepository;
        public DeleteAddressCommandHandler(IAddressLookupRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<string> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var addressEntity = await _addressRepository.GetByIdAsync(request.Id);

                await _addressRepository.DeleteAsync(addressEntity);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Lookup information has been deleted!";
        }
    }
}
