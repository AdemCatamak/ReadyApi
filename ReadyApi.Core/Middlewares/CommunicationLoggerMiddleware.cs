using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReadyApi.Core.Middlewares
{
    public class CommunicationLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CommunicationLoggerMiddleware> _logger;
        private readonly CommunicationLoggerMiddlewareOptions _options;

        public CommunicationLoggerMiddleware(RequestDelegate next, ILogger<CommunicationLoggerMiddleware> logger, IOptions<CommunicationLoggerMiddlewareOptions> options)
        {
            _options = options.Value;
            _next = next;
            _logger = logger;
        }

        public string GetCorrelationId(HttpContext httpContext)
        {
            return httpContext.TraceIdentifier;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string correlationId = GetCorrelationId(httpContext);
            string requestAsString = await httpContext.Request.Stringfy();
            string message = $"[{nameof(CommunicationLoggerMiddleware)}] : {correlationId}{Environment.NewLine}" +
                             $"{requestAsString}";
            _logger.Log(_options.LogLevel, message);

            httpContext.Response.OnCompleted(() =>
                                             {
                                                 message = $"[{nameof(CommunicationLoggerMiddleware)}] : {correlationId}{Environment.NewLine}" +
                                                           $"{httpContext.Response.StatusCode})";
                                                 _logger.Log(_options.LogLevel, message);

                                                 return Task.CompletedTask;
                                             });
            await _next.Invoke(httpContext);
        }
    }

    public class CommunicationLoggerMiddlewareOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
    }

    public static class CommunicationLoggerMiddlewareExtensions
    {
        [Obsolete("UseComminucationLoggerMiddleware should be used")]
        public static void UseComminucationLogger(this IApplicationBuilder app, ILogger<CommunicationLoggerMiddleware> logger, CommunicationLoggerMiddlewareOptions communicationLoggerMiddlewareOptions = null)
        {
            UseComminucationLoggerMiddleware(app, logger, communicationLoggerMiddlewareOptions);
        }

        public static void UseComminucationLoggerMiddleware(this IApplicationBuilder app, ILogger<CommunicationLoggerMiddleware> logger, CommunicationLoggerMiddlewareOptions communicationLoggerMiddlewareOptions = null)
        {
            communicationLoggerMiddlewareOptions = communicationLoggerMiddlewareOptions ?? new CommunicationLoggerMiddlewareOptions();
            app.UseMiddleware<CommunicationLoggerMiddleware>(logger, Options.Create(communicationLoggerMiddlewareOptions));
        }
    }
}