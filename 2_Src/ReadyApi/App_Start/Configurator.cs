using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Alternatives.Extensions;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using RapidLogger;
using ReadyApi.Handlers;
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
            List<Type> logEngineTypes = ReflectionExtensions.GetInheritedTypes(typeof(ILogEngine))
                                                      .ToList();

            List<ILogEngine> logEngines = new List<ILogEngine>();
            foreach (Type logEngineType in logEngineTypes)
            {
                try
                {
                    ILogEngine logEngine = ReflectionExtensions.CreateInstance(logEngineType) as ILogEngine;
                    logEngines.Add(logEngine);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Default ILogEngine Creation Error{Environment.NewLine}{e.Serialize()}");
                }
            }

            LoggerMaestro loggerMaestro = new LoggerMaestro();
            logEngines.ForEach(engine => loggerMaestro.AddLogger(nameof(engine), engine));

            containerBuilder.RegisterInstance(loggerMaestro)
                            .As<LoggerMaestro>()
                            .AsSelf()
                            .PropertiesAutowired()
                            .SingleInstance();

            containerBuilder.RegisterApiControllers(AppDomain.CurrentDomain.GetAssemblies());

            containerBuilder.Register(c => new GeneralExceptionHandler(c.Resolve<LoggerMaestro>()))
                            .AsWebApiExceptionFilterFor<ApiController>()
                            .InstancePerRequest();
        }

        public void DetectDependencies(ref ContainerBuilder containerBuilder)
        {
            List<Type> containerRegisters = ReflectionExtensions.GetInheritedTypes(typeof(IIoCContainer))
                                                          .ToList();

            foreach (Type type in containerRegisters)
            {
                Console.WriteLine($"{Environment.NewLine}{type.Name} - Registration Start");
                try
                {
                    IIoCContainer customContainer = ReflectionExtensions.CreateInstance(type) as IIoCContainer;
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