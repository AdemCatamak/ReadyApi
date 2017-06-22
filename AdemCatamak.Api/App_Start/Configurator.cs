using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using System.Web.Http;
using AdemCatamak.Api.Handlers;
using AdemCatamak.Api.Model;
using AdemCatamak.DAL;
using AdemCatamak.Logger;
using AdemCatamak.Utilities;
using Autofac.Integration.WebApi;

namespace AdemCatamak.Api
{
    internal class Configurator
    {
        public HttpConfiguration Configure()
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            SwaggerConfig.Register(config);

            config.EnsureInitialized();

            return config;
        }

        public void InjectDependencies(ref ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<LogWrapper>()
                            .As<ILogWrapper>()
                            .PropertiesAutowired()
                            .InstancePerRequest();

            containerBuilder.RegisterApiControllers(AppDomain.CurrentDomain.GetAssemblies());

            containerBuilder.Register(c => new GeneralExceptionHandler(c.Resolve<ILogWrapper>()))
                            .AsWebApiExceptionFilterFor<ApiController>()
                            .InstancePerRequest();
        }

        public void DetectDependencies(ref ContainerBuilder containerBuilder)
        {
            List<Type> containerRegisters = ModelCollector.GetInheritedTypes(typeof(IIoCContainer))
                                                          .ToList();

            foreach (Type type in containerRegisters)
            {
                Console.WriteLine($"{Environment.NewLine}{type.Name} - Registration Start");
                try
                {
                    IIoCContainer customContainer = Extensions.CreateInstance(type) as IIoCContainer;
                    if (customContainer != null)
                    {
                        containerBuilder = customContainer.Register(containerBuilder);
                    }

                    Console.WriteLine($"{type.Name} - Registration Finished{Environment.NewLine}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Configurator Type Creation Error : {type.Name}{Environment.NewLine}" +
                                      $"Exception Message : {ex.Message}{Environment.NewLine}" +
                                      $"Exception : {ex}");
                }
            }
        }
    }
}