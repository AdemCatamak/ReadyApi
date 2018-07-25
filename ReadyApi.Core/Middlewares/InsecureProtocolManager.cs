using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ReadyApi.Core.Middlewares
{
    public class InsecureProtocolManager
    {
        private readonly RequestDelegate _next;
        private readonly InsecureProtocolOptions _options;

        public InsecureProtocolManager(RequestDelegate next, IOptions<InsecureProtocolOptions> options)
        {
            _options = options.Value;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.IsHttps && !_options.InsecureProtocolAllowed)
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

    public class InsecureProtocolOptions
    {
        public bool InsecureProtocolAllowed { get; set; } = false;
    }

    public static class InsecureProtocolExtensions
    {
        public static void UseInsecureProtocol(this IApplicationBuilder app, InsecureProtocolOptions insecureProtocolOptions)
        {
            app.UseMiddleware<InsecureProtocolManager>(Options.Create(insecureProtocolOptions));
        }
    }
}