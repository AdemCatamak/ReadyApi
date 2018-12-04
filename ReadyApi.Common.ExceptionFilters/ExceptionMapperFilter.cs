using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReadyApi.Common.Model;

namespace ReadyApi.Common.ExceptionFilters
{
    public class ExceptionMapperFilter : ExceptionFilterAttribute
    {
        private readonly Type _exceptionType;
        private readonly HttpStatusCode _returnStatusCode;
        private readonly string _errorMessage;

        public ExceptionMapperFilter(Type exceptionType, HttpStatusCode returnStatusCode, string errorMessage = "")
        {
            _exceptionType = exceptionType;
            _returnStatusCode = returnStatusCode;
            _errorMessage = errorMessage;
        }

        public override void OnException(ExceptionContext context)
        {
            string traceId = context.HttpContext.TraceIdentifier;
            var basicErrorResponse = new BasicErrorResponse();
            if (context.Exception.GetType() == _exceptionType)
            {
                basicErrorResponse.AddErrorMessage(_errorMessage);
                basicErrorResponse.SetCorrelationId(traceId);

                context.Result = new ObjectResult(basicErrorResponse)
                                 {
                                     StatusCode = (int) _returnStatusCode
                                 };

                return;
            }

            context.ExceptionHandled = false;
        }
    }
}