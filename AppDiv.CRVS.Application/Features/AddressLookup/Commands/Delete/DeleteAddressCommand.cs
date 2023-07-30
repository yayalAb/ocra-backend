using AppDiv.CRVS.Application.Common;
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
    // Customer create command with BaseResponse response
    public class DeleteAddressCommand : IRequest<BaseResponse>
    {
        public Guid[] Id { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, BaseResponse>
    {
        private readonly IAddressLookupRepository _addressRepository;
        public DeleteAddressCommandHandler(IAddressLookupRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<BaseResponse> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                foreach (Guid id in request.Id)
                {
                    await _addressRepository.DeleteAsync(id);

                }
                await _addressRepository.SaveChangesAsync(cancellationToken);

                response.Deleted("Address");

            }
            catch (Exception exp)
            {
                response.BadRequest("Unable to delete the specified address.");
            }
            return response;
        }
    }
}
