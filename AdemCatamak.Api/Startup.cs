using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;

namespace AdemCatamak.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            Configurator configurator = new Configurator();
            HttpConfiguration config = configurator.Configure();

            ContainerBuilder containerBuilder = new ContainerBuilder();

            configurator.InjectDependencies(config, ref containerBuilder);
            configurator.DetectDependencies(ref containerBuilder);

            IContainer container = containerBuilder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(config);
            appBuilder.UseWebApi(config);
        }
    }
}