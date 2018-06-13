using System.Web.Http;
using Newtonsoft.Json;
using Owin;

namespace ReadyApi.UnitTest.GeneralExeptionHandlerTest
{
    public class GeneralExceptionStartup
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


            config.UseGlobalExceptionHandler();

            config.EnsureInitialized();

            appBuilder.UseWebApi(config);
        }
    }
}