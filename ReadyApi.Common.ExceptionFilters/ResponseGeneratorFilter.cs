using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReadyApi.Common.Exceptions.CustomExceptions;
using ReadyApi.Common.Exceptions.ProbDetails;
using ReadyApi.Common.Model;

namespace ReadyApi.Common.ExceptionFilters
{
    public class ResponseGeneratorFilter : IExceptionFilter
    {
        private readonly BasicErrorResponse _basicErrorResponse;

        public ResponseGeneratorFilter(BasicErrorResponse basicErrorResponse = null)
        {
            _basicErrorResponse = basicErrorResponse ?? new BasicErrorResponse();
        }

        public void OnException(ExceptionContext context)
        {
            string traceId = context.HttpContext.TraceIdentifier;

            var httpStatusCode = HttpStatusCode.InternalServerError;
            string message = "Unexpected Error Occurs";

            if (context.Exception is CustomException customException)
            {
                message = customException.ProblemDetail.ToString();

                if (customException.ProblemDetail is ApiProblemDetails apiProblemDetails)
                {
                    httpStatusCode = apiProblemDetails.StatusCode;
                }
            }

            _basicErrorResponse.SetCorrelationId(traceId);
            _basicErrorResponse.AddErrorMessage(message);

            context.Result = new ObjectResult(_basicErrorResponse)
                             {
                                 StatusCode = (int) httpStatusCode
                             };
        }
    }
}