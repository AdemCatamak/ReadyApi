using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using AdemCatamak.Api.Model;

namespace AdemCatamak.Api.Test.TestBasicAuthenticationChecker
{
    public class AuthenticationCheckerTest : IAuthenticationChecker
    {
        public IPrincipal Check(string userName, string password, CancellationToken cancellationToken)
        {
            IPrincipal principal = null;
            if (userName == "adem" && password == "1")
            {
                GenericIdentity identity = new GenericIdentity(userName);
                identity.AddClaim(new Claim(ClaimTypes.Name, userName));
                principal = new ClaimsPrincipal(identity);
            }

            return principal;
        }
    }
}