using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReadyApi.Core.Middlewares
{
    public class ExceptionLogger
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ExceptionLoggerOptions _options;

        public ExceptionLogger(RequestDelegate next, ILogger logger, IOptions<ExceptionLoggerOptions> options)
        {
            _options = options.Value;
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                string requestAsString = await FormatRequest(httpContext.Request);
                string message = $"Request = {requestAsString}{Environment.NewLine}" +
                          $"Exception = {ex}";
                Log(message, _options.LogLevel);
            }

        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            Stream body = request.Body;
            request.EnableRewind();

            byte[] buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            string bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body = body;

            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private void Log(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.LogTrace(message);
                    break;
                case LogLevel.Debug:
                    _logger.LogDebug(message);
                    break;
                case LogLevel.Information:
                    _logger.LogInformation(message);
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(message);
                    break;
                case LogLevel.Error:
                    _logger.LogError(message);
                    break;
                case LogLevel.Critical:
                    _logger.LogCritical(message);
                    break;
                default:
                    _logger.LogInformation(message);
                    break;
            }
        }
    }

    public class ExceptionLoggerOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Error;
    }

    public static class ExceptionLoggerExtensions
    {
        public static void UseExceptionLogger(this IApplicationBuilder app, ILogger logger, ExceptionLoggerOptions exceptionLoggerOptions = null)
        {
            exceptionLoggerOptions = exceptionLoggerOptions ?? new ExceptionLoggerOptions();
            app.UseMiddleware<ExceptionLogger>(logger, Options.Create(exceptionLoggerOptions));
        }
    }
}