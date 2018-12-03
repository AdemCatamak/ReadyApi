using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ReadyApi.Common.Exceptions;
using ReadyApi.Common.Exceptions.CustomExceptions;
using ReadyApi.Common.Exceptions.ProbDetails;

namespace ReadyApi.Common.ExceptionFilters
{
    public class ExceptionLoggerFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionLoggerFilter> _logger;

        public ExceptionLoggerFilter(ILogger<ExceptionLoggerFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            string message = "Unhandled Exception Occurs";
            var logLevel = LogLevel.Error;

            if (context.Exception is CustomException customException)
            {
                message = customException.ToString();
                logLevel = DecideLogLevel(customException);
            }

            _logger.Log(logLevel, new EventId((int) logLevel, context.Exception.GetType().FullName), typeof(ExceptionLoggerFilter), context.Exception, (type, exception) => message);

            context.ExceptionHandled = false;
        }

        private LogLevel DecideLogLevel(CustomException customException)
        {
            var logLevel = LogLevel.Error;

            if (customException.ProblemDetail is ApiProblemDetails apiProblemDetails)
            {
                if ((int) apiProblemDetails.StatusCode < (int) HttpStatusCode.BadRequest)
                {
                    logLevel = LogLevel.Information;
                }
                else if ((int) apiProblemDetails.StatusCode < (int) HttpStatusCode.InternalServerError)
                {
                    logLevel = LogLevel.Warning;
                }
            }
            else if (customException.ExceptionTags != null && customException.ExceptionTags.Contains(ExceptionTags.ClientsFault))
            {
                logLevel = LogLevel.Warning;
            }

            return logLevel;
        }
    }
}