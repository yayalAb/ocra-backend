using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IMergeAndSplitAddressService
    {
        public Task<BaseResponse> MergeAndSplitAddress(List<AddAddressRequest> Address, CancellationToken cancellationToken);

    }
}
