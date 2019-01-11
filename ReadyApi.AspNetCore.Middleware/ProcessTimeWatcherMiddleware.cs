using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReadyApi.AspNetCore.Middleware
{
    public class ProcessTimeWatcherMiddleware
    {
        private readonly ProcessTimeWatcherOptions _options;
        private readonly RequestDelegate _next;
        private readonly ILogger<ProcessTimeWatcherMiddleware> _logger;
        private readonly Func<HttpContext, string> _getCorrelationId;

        public ProcessTimeWatcherMiddleware(RequestDelegate next, ILogger<ProcessTimeWatcherMiddleware> logger, IOptions<ProcessTimeWatcherOptions> options)
        {
            _options = options.Value;
            _next = next;
            _logger = logger;
            _getCorrelationId = _options.GetCorrelationId ?? GetCorrelationId;
        }

        private string GetCorrelationId(HttpContext httpContext)
        {
            return httpContext.TraceIdentifier;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string endpointName = httpContext.Request.Path;
            string verb = httpContext.Request.Method;

            string correlationId = _getCorrelationId(httpContext);

            var watch = new Stopwatch();
            try
            {
                watch.Start();
                await _next.Invoke(httpContext);
            }
            finally
            {
                watch.Stop();
                string message = $"[{nameof(ProcessTimeWatcherMiddleware)}] : [{verb}] - [{endpointName}] - [traceId:{correlationId}] - {watch.ElapsedMilliseconds} Ms";
                _logger.Log(_options.LogLevel, new EventId((int) _options.LogLevel), typeof(ProcessTimeWatcherMiddleware), null, (type, exception) => message);
            }
        }
    }

    public class ProcessTimeWatcherOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public Func<HttpContext, string> GetCorrelationId;

    }

    public static class ProcessTimeWatcherMiddlewareExtensions
    {
        public static void UseProcessTimeWatcherMiddleware(this IApplicationBuilder app, ILogger<ProcessTimeWatcherMiddleware> logger, ProcessTimeWatcherOptions processTimeWatcherOptions = null)
        {
            processTimeWatcherOptions = processTimeWatcherOptions ?? new ProcessTimeWatcherOptions();
            app.UseMiddleware<ProcessTimeWatcherMiddleware>(logger, Options.Create(processTimeWatcherOptions));
        }
    }
}