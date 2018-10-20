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

            string requestAsString;
            try
            {
                requestAsString = await FormatRequest(httpContext.Request);
            }
            catch (Exception e)
            {
                requestAsString = $"Read error [{e.Message}]";
            }

            string message = $"[{nameof(CommunicationLoggerMiddleware)}] : Request is arrived. Guid: {correlationId}{Environment.NewLine}{requestAsString}";
            _logger.Log(_options.LogLevel, message);

            try
            {
                await _next.Invoke(httpContext);
            }
            finally
            {
                string responseAsString;
                try
                {
                    responseAsString = await FormatResponse(httpContext.Response);
                }
                catch (Exception e)
                {
                    responseAsString = $"Read error [{e.Message}]";
                }

                message = $"[{nameof(CommunicationLoggerMiddleware)}] : Response is sent. Guid: {correlationId}{Environment.NewLine}{responseAsString})";
                _logger.Log(_options.LogLevel, message);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            Stream body = request.Body;
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
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
    }

    public class CommunicationLoggerMiddlewareOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
        public string CorrelationIdHeaderName { get; set; } = CorrelationIdMiddlewareOptions.DEFAULT_HEADER;
    }

    public static class CommunicationLoggerMiddlewareExtensions
    {
        public static void UseComminucationLogger(this IApplicationBuilder app, ILogger<CommunicationLoggerMiddleware> logger, CommunicationLoggerMiddlewareOptions communicationLoggerMiddlewareOptions = null)
        {
            communicationLoggerMiddlewareOptions = communicationLoggerMiddlewareOptions ?? new CommunicationLoggerMiddlewareOptions();
            app.UseMiddleware<CommunicationLoggerMiddleware>(logger, Options.Create(communicationLoggerMiddlewareOptions));
        }
    }
}