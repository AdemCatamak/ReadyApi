using System;
using System.Net;
using Alternatives.CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ReadyApi.Core.ExceptionFilter.Response;

namespace ReadyApi.Core.ExceptionFilter
{
    public class GeneralExceptionFilter : IExceptionFilter
    {
        private BasicErrorResponse _basicErrorResponse;
        private readonly ILogger _logger;

        public GeneralExceptionFilter(ILoggerFactory loggerFactory, BasicErrorResponse basicErrorResponse = null)
        {
            _basicErrorResponse = basicErrorResponse;
            _logger = loggerFactory.CreateLogger(nameof(GeneralExceptionFilter));
        }

        public void OnException(ExceptionContext context)
        {
            _basicErrorResponse = _basicErrorResponse ?? new BasicErrorResponse();
            HttpStatusCode resultHttpStatusCode;
            if (context.Exception is CustomApiException customApiException)
            {
                _basicErrorResponse.AddErrorMessage(customApiException.FriendlyMessage);
                resultHttpStatusCode = customApiException.ReturnHttpStatusCode;
            }
            else
            {
                _basicErrorResponse.AddErrorMessage("Unexpected error occured");
                resultHttpStatusCode = HttpStatusCode.InternalServerError;
            }

            context.Result = new ObjectResult(_basicErrorResponse)
                             {
                                 StatusCode = (int) resultHttpStatusCode,
                             };

            DoLogging(context.Exception, resultHttpStatusCode);
        }

        private void DoLogging(Exception contextException, HttpStatusCode resultHttpStatusCode)
        {
            if ((int) resultHttpStatusCode < 400)
            {
                _logger.LogInformation(contextException, contextException.Message);
            }
            else if ((int) resultHttpStatusCode < 500)
            {
                _logger.LogWarning(contextException, contextException.Message);
            }
            else
            {
                _logger.LogError(contextException, contextException.Message);
            }
        }
    }
}