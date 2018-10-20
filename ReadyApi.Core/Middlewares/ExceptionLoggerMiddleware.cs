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
            string correlationId = GetCorrelationId(httpContext);
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                string requestAsString;
                try
                {
                    requestAsString = await FormatRequest(httpContext.Request);
                }
                catch (Exception e)
                {
                    requestAsString = string.Empty;
                }

                string message = $"[{nameof(ExceptionLoggerMiddleware)}] - {correlationId}{Environment.NewLine}" +
                                 $"Request = {requestAsString}{Environment.NewLine}" +
                                 $"Exception = {ex}";

                _logger.Log(_middlewareOptions.LogLevel, message);
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
    }

    public class ExceptionLoggerMiddlewareOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Error;
    }

    public static class ExceptionLoggerMiddlewareExtensions
    {
        public static void UseExceptionLogger(this IApplicationBuilder app, ILogger<ExceptionLoggerMiddleware> logger, ExceptionLoggerMiddlewareOptions exceptionLoggerMiddlewareOptions = null)
        {
            exceptionLoggerMiddlewareOptions = exceptionLoggerMiddlewareOptions ?? new ExceptionLoggerMiddlewareOptions();
            app.UseMiddleware<ExceptionLoggerMiddleware>(logger, Options.Create(exceptionLoggerMiddlewareOptions));
        }
    }
}