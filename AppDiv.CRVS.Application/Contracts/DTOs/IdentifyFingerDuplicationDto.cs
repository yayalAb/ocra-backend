using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class IdentifyFingerDuplicationDto
    {
        public string clientKey { get; set; }
        public int instanceID { get; set; }
        public string sequenceNo { get; set; }
        public string operationName { get; set; }
        public string operationStatus { get; set; }
        public string operationResult { get; set; }
        public string message { get; set; }
        public bestResultDto bestResult { get; set; }
        public detailResultDto[] detailResult { get; set; }
        public PersonalInfoDTO? DuplicatedPerson { get; set; }
    }
}