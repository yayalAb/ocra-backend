using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IWorkflowService
    {
        public int GetLastWorkflow(string workflowType);
    }
}