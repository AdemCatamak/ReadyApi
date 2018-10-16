using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ReadyApi.Core.Middlewares
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
                context.TraceIdentifier = Guid.NewGuid().ToString();
            }

            try
            {
                await _next.Invoke(context);
            }
            finally
            {
                if (_middlewareOptions.IncludeInResponse)
                {
                    context.Response.Headers.Add(_middlewareOptions.Header, new[] {context.TraceIdentifier});
                }
            }
        }
    }

    public class CorrelationIdMiddlewareOptions
    {
        public const string DEFAULT_HEADER = "X-Correlation-Id";

        public string Header { get; set; } = DEFAULT_HEADER;

        public bool IncludeInResponse { get; set; } = true;
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