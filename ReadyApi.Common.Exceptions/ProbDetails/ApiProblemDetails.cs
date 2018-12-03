using System.Net;

namespace ReadyApi.Common.Exceptions.ProbDetails
{
    public class ApiProblemDetails : ProblemDetails
    {
        public HttpStatusCode StatusCode { get; }

        public ApiProblemDetails(string title, HttpStatusCode statusCode)
            : this(title, statusCode, string.Empty)
        {
        }

        public ApiProblemDetails(string title, HttpStatusCode statusCode, string detail)
            : base(title, detail)
        {
            StatusCode = statusCode;
        }
    }
}