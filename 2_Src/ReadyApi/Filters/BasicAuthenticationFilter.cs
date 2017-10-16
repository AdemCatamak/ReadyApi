using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using ReadyApi.Model;

namespace ReadyApi.Filters
{
    public class BasicAuthenticationFilter : IAuthenticationFilter
    {
        private readonly IAuthenticationChecker _authenticationChecker;
        private readonly IUserRoleStore _userRoleStore;

        public BasicAuthenticationFilter(IAuthenticationChecker authenticationChecker, IUserRoleStore userRoleStore = null)
        {
            _authenticationChecker = authenticationChecker;
            _userRoleStore = userRoleStore;
        }

        public bool AllowMultiple => false;

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            if (authorization == null)
            {
                return Task.FromResult(0);
            }

            if (authorization.Scheme != "Basic")
            {
                return Task.FromResult(0);
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return Task.FromResult(0);
            }

            Tuple<string, string> userNameAndPasword = ExtractUserNameAndPassword(authorization.Parameter);
            if (userNameAndPasword == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                return Task.FromResult(0);
            }

            string userName = userNameAndPasword.Item1;
            string password = userNameAndPasword.Item2;

            IPrincipal principal = null;

            bool hasUserRight = _authenticationChecker.Check(userName, password);

            if (hasUserRight)
            {
                string[] roles = { };
                GenericIdentity identity = new GenericIdentity(userName);

                if (_userRoleStore != null)
                {
                    IEnumerable<string> roleInfo = _userRoleStore.GetRoles(userName);
                    roles = roleInfo?.ToArray() ?? new string[] { };

                    foreach (string role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                }

                principal = new GenericPrincipal(identity, roles);
            }

            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
                return Task.FromResult(0);
            }

            context.Principal = principal;

            return Task.FromResult(0);
        }


        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            AuthenticationHeaderValue challenge = new AuthenticationHeaderValue("Basic");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }


        private Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
        {
            authorizationParameter = Encoding.Default.GetString(Convert.FromBase64String(authorizationParameter));

            string[] tokens = authorizationParameter.Split(':');
            return tokens.Length != 2
                       ? null
                       : new Tuple<string, string>(tokens[0], tokens[1]);
        }


        private class AddChallengeOnUnauthorizedResult : IHttpActionResult
        {
            public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
            {
                _challenge = challenge;
                _innerResult = innerResult;
            }

            private AuthenticationHeaderValue _challenge { get; }

            private IHttpActionResult _innerResult { get; }

            public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage response = await _innerResult.ExecuteAsync(cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (response.Headers.WwwAuthenticate.All(h => h.Scheme != _challenge.Scheme))
                    {
                        response.Headers.WwwAuthenticate.Add(_challenge);
                    }
                }

                return response;
            }
        }

        private class AuthenticationFailureResult : IHttpActionResult
        {
            private string _reasonPhrase { get; }
            private HttpRequestMessage _request { get; }

            public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
            {
                _reasonPhrase = reasonPhrase;
                _request = request;
            }


            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute());
            }

            private HttpResponseMessage Execute()
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                                               {
                                                   RequestMessage = _request,
                                                   ReasonPhrase = _reasonPhrase,
                                               };
                return response;
            }
        }
    }
}