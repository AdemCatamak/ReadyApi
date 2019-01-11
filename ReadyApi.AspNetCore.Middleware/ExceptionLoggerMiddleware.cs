using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReadyApi.AspNetCore.Middleware
{
    public class ExceptionLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggerMiddleware> _logger;
        private readonly ExceptionLoggerMiddlewareOptions _middlewareOptions;
        private readonly Func<HttpContext, string> _getCorrelationId;

        public ExceptionLoggerMiddleware(RequestDelegate next, ILogger<ExceptionLoggerMiddleware> logger, IOptions<ExceptionLoggerMiddlewareOptions> options)
        {
            _middlewareOptions = options.Value;
            _next = next;
            _logger = logger;
            _getCorrelationId = _middlewareOptions.GetTraceId ?? GetCorrelationId;
        }

        private string GetCorrelationId(HttpContext httpContext)
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
                string correlationId = _getCorrelationId(httpContext);
                string requestAsString = await httpContext.Request.Stringfy();
                string message = $"[{nameof(ExceptionLoggerMiddleware)}] - [traceId:{correlationId}]{Environment.NewLine}" +
                                 $"Request = {requestAsString}{Environment.NewLine}" +
                                 $"Exception = {ex}";

                _logger.Log(_middlewareOptions.LogLevel, new EventId((int) _middlewareOptions.LogLevel, ex.GetType().FullName), typeof(ExceptionLoggerMiddleware), ex, (type, exception) => message);

                throw;
            }
        }
    }

    public class ExceptionLoggerMiddlewareOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Error;
        public readonly Func<HttpContext, string> GetTraceId = null;
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