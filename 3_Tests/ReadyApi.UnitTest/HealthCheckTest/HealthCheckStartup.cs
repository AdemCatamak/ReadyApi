using System.Web.Http;
using Newtonsoft.Json;
using Owin;
using ReadyApi.UnitTest.BasicAuthenticationTest;

namespace ReadyApi.UnitTest.HealthCheckTest
{
    public class HealthCheckStartup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings()
                                                                 {
                                                                     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                                     ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                                                                 };


            config.UseGlobalExceptionHandler()
                  .UseBasicAuthenticationFilter(new AuthenticationChecker(), new UserRoleStore());

            config.EnsureInitialized();

            appBuilder.UseWebApi(config);
        }
    }
}