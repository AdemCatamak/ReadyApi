using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ReadyApi.Core.Middlewares
{
    public class InsecureProtocolManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly InsecureProtocolMiddlewareOptions _middlewareOptions;

        public InsecureProtocolManagerMiddleware(RequestDelegate next, IOptions<InsecureProtocolMiddlewareOptions> options)
        {
            _middlewareOptions = options.Value;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.IsHttps && !_middlewareOptions.InsecureProtocolAllowed)
            {
                const string insecureProtocolMessage = "Request is HTTP, Basic Authentication will not respond.";
                httpContext.Response.StatusCode = (int) HttpStatusCode.PreconditionFailed;
                byte[] encodedResponseText = Encoding.UTF8.GetBytes(insecureProtocolMessage);
                httpContext.Response.Body.Write(encodedResponseText, 0, encodedResponseText.Length);
                return;
            }

            await _next.Invoke(httpContext);
        }
    }

    public class InsecureProtocolMiddlewareOptions
    {
        public bool InsecureProtocolAllowed { get; set; }
    }

    public static class InsecureProtocolMiddlewareExtensions
    {
        public static void UseInsecureProtocol(this IApplicationBuilder app, InsecureProtocolMiddlewareOptions insecureProtocolMiddlewareOptions = null)
        {
            insecureProtocolMiddlewareOptions = insecureProtocolMiddlewareOptions ?? new InsecureProtocolMiddlewareOptions();
            app.UseMiddleware<InsecureProtocolManagerMiddleware>(Options.Create(insecureProtocolMiddlewareOptions));
        }
    }
}