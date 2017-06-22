using System.Security.Principal;
using System.Threading;

namespace AdemCatamak.Api.Model
{
    public interface IAuthenticationChecker
    {
        /// <summary>
        /// If username and password is valid, it should be return IPrincipal instance, else function must return null
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IPrincipal Check(string userName, string password, CancellationToken cancellationToken);
    }
}