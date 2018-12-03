using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace ReadyApi.AspNetCore.BasicAuth
{
    public class BasicAuthHandler : AuthenticationHandler<BasicAuthOptions>
    {
        public BasicAuthHandler(IOptionsMonitor<BasicAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (!authorizationHeader.StartsWith(BasicAuthDefaults.AUTHENTICATION_SCHEME + ' ', StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            string encodedCredentials = authorizationHeader.Substring(BasicAuthDefaults.AUTHENTICATION_SCHEME.Length).Trim();

            if (string.IsNullOrEmpty(encodedCredentials))
            {
                const string noCredentialsMessage = "No credentials";
                return Task.FromResult(AuthenticateResult.Fail(noCredentialsMessage));
            }

            string decodedCredentials;
            try
            {
                decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to decode credentials : {encodedCredentials}", ex);
            }

            int delimiterIndex = decodedCredentials.IndexOf(':');
            if (delimiterIndex == -1)
            {
                const string missingDelimiterMessage = "Invalid credentials, missing delimiter.";
                Logger.LogInformation(missingDelimiterMessage);
                return Task.FromResult(AuthenticateResult.Fail(missingDelimiterMessage));
            }

            string username = decodedCredentials.Substring(0, delimiterIndex);
            string password = decodedCredentials.Substring(delimiterIndex + 1);

            ClaimsPrincipal claimsPrincipal = Options.ExecuteBasicAuthHandler(username, password);

            if (claimsPrincipal == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("User not found"));
            }

            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            string headerValue = Request.Headers["Authorization"];
            Response.Headers.Append(HeaderNames.WWWAuthenticate, headerValue);

            return Task.CompletedTask;
        }
    }
}