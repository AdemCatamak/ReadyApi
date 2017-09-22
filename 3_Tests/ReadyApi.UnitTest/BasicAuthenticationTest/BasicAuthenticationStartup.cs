using System.Web.Http;
using Newtonsoft.Json;
using Owin;

namespace ReadyApi.UnitTest.BasicAuthenticationTest
{
    public class BasicAuthenticationStartup
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