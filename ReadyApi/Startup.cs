using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;
using Alternatives;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using RapidLogger;
using ReadyApi.Filters;
using ReadyApi.Handlers;
using ReadyApi.Model;

namespace ReadyApi
{
    public class Startup
    {
        public static IContainer IoCContainer { get; set; }
        private static HttpConfiguration HttpConfig { get; set; }

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

        public static void UpdateConfiguration(IStartupConfigure configurator)
        {
            ContainerBuilder newBuilder = new ContainerBuilder();

            configurator.Configure(HttpConfig);
            newBuilder.RegisterWebApiFilterProvider(HttpConfig);

#pragma warning disable 618
            newBuilder.Update(IoCContainer);
#pragma warning restore 618
        }

        public static void AddFilter(IFilter filter)
        {
            ContainerBuilder newBuilder = new ContainerBuilder();

            HttpConfig.Filters.Add(filter);
            newBuilder.RegisterWebApiFilterProvider(HttpConfig);

#pragma warning disable 618
            newBuilder.Update(IoCContainer);
#pragma warning restore 618
        }

        public static void RemoveFilter(IFilter filter)
        {
            ContainerBuilder newBuilder = new ContainerBuilder();

            HttpConfig.Filters.Remove(filter);
            newBuilder.RegisterWebApiFilterProvider(HttpConfig);

#pragma warning disable 618
            newBuilder.Update(IoCContainer);
#pragma warning restore 618
        }

        public static void UseBasicAuthentication(IAuthenticationChecker authenticationChecker, IUserRoleStore userRoleStore = null)
        {
            BasicAuthenticationFilter basicAuthenticationFilter = new BasicAuthenticationFilter(authenticationChecker, userRoleStore);
            AddFilter(basicAuthenticationFilter);
        }
    }
}