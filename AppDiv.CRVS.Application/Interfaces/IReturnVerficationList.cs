using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces
{

    public interface IReturnVerficationList
    {
        public Task<IQueryable<Event>> GetVerficationRequestedCertificateList(bool isVerfication=true);

    }

}