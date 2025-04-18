using System.Net;

namespace Common.Models
{
    public class ResponseModel : BaseResponseModel
    {
        public string Status { get; set; }
    }
    public class BaseResponseModel 
    {
        public object Data { get; set; }
        public string Message { get; set; }
        public HttpStatusCode statusCode { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
   
}
