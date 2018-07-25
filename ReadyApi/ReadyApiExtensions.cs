using System.Web.Http;
using RapidLogger;
using ReadyApi.Filters;
using ReadyApi.Handlers;
using ReadyApi.Model;

namespace ReadyApi
{
    public static class ReadyApiExtensions
    {
        public static HttpConfiguration UseGlobalExceptionHandler(this HttpConfiguration configuration, LoggerMaestro loggerMaestro)
        {
            GeneralExceptionHandler generalExceptionHandler = new GeneralExceptionHandler(loggerMaestro);

            configuration.Filters.Add(generalExceptionHandler);

            return configuration;
        }

        public static HttpConfiguration UseBasicAuthenticationFilter(this HttpConfiguration configuration, IAuthenticationChecker authenticationChecker, IUserRoleStore userRoleStore = null)
        {
            BasicAuthenticationFilter basicAuthenticationFilter = new BasicAuthenticationFilter(authenticationChecker, userRoleStore);
            configuration.Filters.Add(basicAuthenticationFilter);
            return configuration;
        }
    }
}