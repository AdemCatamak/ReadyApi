using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ReadyApi.Core.Middlewares
{
    public class ProcessTimeWatcherMiddleware
    {
        private readonly ProcessTimeWatcherOptions _options;
        private readonly RequestDelegate _next;
        private readonly ILogger<ProcessTimeWatcherMiddleware> _logger;

        public ProcessTimeWatcherMiddleware(RequestDelegate next, ILogger<ProcessTimeWatcherMiddleware> logger, IOptions<ProcessTimeWatcherOptions> options)
        {
            _options = options.Value;
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string endpointName = httpContext.Request.Path;
            if (!httpContext.Request.Headers.TryGetValue(_options.CorrelationIdHeaderName, out StringValues correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            var watch = new Stopwatch();
            try
            {
                watch.Start();
                await _next.Invoke(httpContext);
            }
            finally
            {
                watch.Stop();
                long elapsedMs = watch.ElapsedMilliseconds;
                _logger.Log(_options.LogLevel, $"{nameof(ProcessTimeWatcherMiddleware)} : [{endpointName}] - [{correlationId}] - {elapsedMs} Ms");
            }
        }
    }

    public class ProcessTimeWatcherOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public string CorrelationIdHeaderName { get; set; } = CorrelationIdMiddlewareOptions.DEFAULT_HEADER;
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