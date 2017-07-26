using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AdemCatamak.Api;
using Alternatives;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using ReadyApi.Model;

namespace ReadyApi
{
    internal class Configurator
    {
        public HttpConfiguration Configure()
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings()
                                                                 {
                                                                     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                                     ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                                                                 };

            SwaggerConfig.Register(config);

            config.EnsureInitialized();

            return config;
        }

        public void InjectDependencies(ref ContainerBuilder containerBuilder)
        {

            containerBuilder.RegisterApiControllers(AppDomain.CurrentDomain.GetAssemblies());

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