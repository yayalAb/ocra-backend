using System.Collections.Generic;

namespace AppDiv.CRVS.Application.Common
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            Success = true;
        }

        public BaseResponse(string message = null)
        {
            Success = true;
            Message = message;
        }

        public BaseResponse(string message, bool success)
        {
            Success = success;
            Message = message;
        }

        public BaseResponse(string message, bool success, int status = 200)
        {
            Success = success;
            Message = message;
            Status = status;
        }


        public bool Success { get; set; }
        public string Message { get; set; }
        public int Status { get; set; } = 200;
        public List<string> ValidationErrors { get; set; }
    }
}