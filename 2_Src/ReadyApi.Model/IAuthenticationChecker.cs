namespace ReadyApi.Model
{
    public interface IAuthenticationChecker
    {
        /// <summary>
        /// If username and password is valid, it should be return IPrincipal instance, else function must return null
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool Check(string userName, string password);
    }
}