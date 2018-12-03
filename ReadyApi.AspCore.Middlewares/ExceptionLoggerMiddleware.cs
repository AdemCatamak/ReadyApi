using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReadyApi.AspCore.Middlewares
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

                _logger.Log(_middlewareOptions.LogLevel, message);

                if (httpContext.Response.HasStarted)
                    return;

                httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsync(ex.Message);
            }
        }
    }

    public class ExceptionLoggerMiddlewareOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Error;
    }

    public static class ExceptionLoggerMiddlewareExtensions
    {
        [Obsolete("UseExceptionLoggerMiddleware extension fuction should be used")]
        public static void UseExceptionLogger(this IApplicationBuilder app, ILogger<ExceptionLoggerMiddleware> logger, ExceptionLoggerMiddlewareOptions exceptionLoggerMiddlewareOptions = null)
        {
            UseExceptionLoggerMiddleware(app, logger, exceptionLoggerMiddlewareOptions);
        }

        public static void UseExceptionLoggerMiddleware(this IApplicationBuilder app, ILogger<ExceptionLoggerMiddleware> logger, ExceptionLoggerMiddlewareOptions exceptionLoggerMiddlewareOptions = null)
        {
            exceptionLoggerMiddlewareOptions = exceptionLoggerMiddlewareOptions ?? new ExceptionLoggerMiddlewareOptions();
            app.UseMiddleware<ExceptionLoggerMiddleware>(logger, Options.Create(exceptionLoggerMiddlewareOptions));
        }
    }
}