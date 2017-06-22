using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;

namespace AdemCatamak.Api
{
    public class Startup
    {
        public static IContainer IoCContainer { get; set; }
        public static HttpConfiguration HttpConfig { get; set; }

        public void Configuration(IAppBuilder appBuilder)
        {
            Configurator configurator = new Configurator();
            HttpConfiguration config = configurator.Configure();

            ContainerBuilder containerBuilder = new ContainerBuilder();

            configurator.InjectDependencies(ref containerBuilder);
            configurator.DetectDependencies(ref containerBuilder);

            containerBuilder.RegisterWebApiFilterProvider(config);
            
            IoCContainer = containerBuilder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(IoCContainer);

            appBuilder.UseAutofacMiddleware(IoCContainer);
            appBuilder.UseAutofacWebApi(config);
            appBuilder.UseWebApi(config);

            HttpConfig = config;
        }

        public static void UpdateConfiguration(HttpConfiguration config)
        {
            ContainerBuilder newBuilder = new ContainerBuilder();
            newBuilder.RegisterWebApiFilterProvider(config);

            newBuilder.Update(IoCContainer);

            HttpConfig = config;
        }
    }
}