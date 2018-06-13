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
    public class CommunicationLogger
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly CommunicationLoggerOptions _options;

        public CommunicationLogger(RequestDelegate next, ILogger logger, IOptions<CommunicationLoggerOptions> options)
        {
            _options = options.Value;
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string guid = Guid.NewGuid().ToString();

            string requestAsString = await FormatRequest(httpContext.Request);
            string message = $"Request is arrived. Guid: {guid}{Environment.NewLine}{requestAsString}";
            Log(message, _options.LogLevel);

            await _next.Invoke(httpContext);

            string responseAsString = await FormatResponse(httpContext.Response);
            message = $"Response is sent. Guid: {guid}{Environment.NewLine}{responseAsString})";
            Log(message, _options.LogLevel);
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

        private async Task<string> FormatResponse(HttpResponse response)
        {
            string text = string.Empty;
            if (response.Body.CanRead)
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                text = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
            }

            return $"Response {text}";
        }

        private void Log(string message, LogLevel logLevel)
        {
            switch (_options.LogLevel)
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

    public class CommunicationLoggerOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
    }

    public static class CommunicationLoggerExtensions
    {
        public static void UseComminucationLogger(this IApplicationBuilder app, ILogger logger, CommunicationLoggerOptions communicationLoggerOptions = null)
        {
            communicationLoggerOptions = communicationLoggerOptions ?? new CommunicationLoggerOptions();
            app.UseMiddleware<CommunicationLogger>(logger, Options.Create(communicationLoggerOptions));
        }
    }
}