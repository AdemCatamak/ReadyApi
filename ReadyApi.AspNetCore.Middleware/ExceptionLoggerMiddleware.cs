using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReadyApi.Common.Exceptions;
using ReadyApi.Common.Exceptions.CustomExceptions;
using ReadyApi.Common.Exceptions.ProbDetails;

namespace ReadyApi.AspNetCore.Middleware
{
    public class ExceptionLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggerMiddleware> _logger;
        private readonly ExceptionLoggerMiddlewareOptions _middlewareOptions;

        public ExceptionLoggerMiddleware(RequestDelegate next, ILogger<ExceptionLoggerMiddleware> logger, IOptions<ExceptionLoggerMiddlewareOptions> options)
        {
            _middlewareOptions = options.Value;
            _next = next;
            _logger = logger;
        }

        public string GetCorrelationId(HttpContext httpContext)
        {
            return httpContext.TraceIdentifier;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                string correlationId = GetCorrelationId(httpContext);
                string requestAsString = await httpContext.Request.Stringfy();
                string message = $"[{nameof(ExceptionLoggerMiddleware)}] - {correlationId}{Environment.NewLine}" +
                                 $"Request = {requestAsString}{Environment.NewLine}" +
                                 $"Exception = {ex}";

                LogLevel logLevel = DecideLogLevel(ex);
                _logger.Log(logLevel, new EventId((int) logLevel, ex.GetType().FullName), typeof(ExceptionLoggerMiddleware), ex, (type, exception) => message);

                throw;
            }
        }

        private LogLevel DecideLogLevel(Exception exception)
        {
            LogLevel logLevel = _middlewareOptions.LogLevel;


            if (exception is CustomException customException)
            {
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
                else if (customException.ExceptionTags != null
                         && customException.ExceptionTags.Contains(ExceptionTags.ClientsFault))
                {
                    logLevel = LogLevel.Warning;
                }
            }

            return logLevel;
        }
    }

    public class ExceptionLoggerMiddlewareOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Error;
    }

    public static class ExceptionLoggerMiddlewareExtensions
    {
        public static void UseExceptionLoggerMiddleware(this IApplicationBuilder app, ILogger<ExceptionLoggerMiddleware> logger, ExceptionLoggerMiddlewareOptions exceptionLoggerMiddlewareOptions = null)
        {
            exceptionLoggerMiddlewareOptions = exceptionLoggerMiddlewareOptions ?? new ExceptionLoggerMiddlewareOptions();
            app.UseMiddleware<ExceptionLoggerMiddleware>(logger, Options.Create(exceptionLoggerMiddlewareOptions));
        }
    }
}