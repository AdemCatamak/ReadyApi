using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ReadyApi.AspNetCore.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CorrelationIdMiddlewareOptions _middlewareOptions;

        public CorrelationIdMiddleware(RequestDelegate next, IOptions<CorrelationIdMiddlewareOptions> options)
        {
            _next = next;
            _middlewareOptions = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(_middlewareOptions.Header, out StringValues correlationId))
            {
                context.TraceIdentifier = correlationId;
            }
            else
            {
                string guid = Guid.NewGuid().ToString();
                DateTime requestTime = DateTime.UtcNow;
                context.TraceIdentifier = $"{requestTime:yyyy-MM-dd-HH-mm}:{guid}";
            }

            context.Response.OnStarting(() =>
                                        {
                                            if (_middlewareOptions.IncludeInResponseHeader)
                                            {
                                                context.Response.Headers.Add(_middlewareOptions.Header, context.TraceIdentifier);
                                            }

                                            return Task.CompletedTask;
                                        });

            await _next.Invoke(context);
        }
    }

    public class CorrelationIdMiddlewareOptions
    {
        public const string DEFAULT_HEADER = "x-correlation-id";

        public string Header { get; set; } = DEFAULT_HEADER;

        public bool IncludeInResponseHeader { get; set; } = true;
    }

    public static class CorrelationIdMiddlewareExtensions
    {
        public static void UseCorrelationIdMiddleware(this IApplicationBuilder app, CorrelationIdMiddlewareOptions correlationIdMiddlewareOptions = null)
        {
            correlationIdMiddlewareOptions = correlationIdMiddlewareOptions ?? new CorrelationIdMiddlewareOptions();
            app.UseMiddleware<CorrelationIdMiddleware>(Options.Create(correlationIdMiddlewareOptions));
        }
    }
}