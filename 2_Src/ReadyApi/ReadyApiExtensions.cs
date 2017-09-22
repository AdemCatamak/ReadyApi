using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Alternatives.Extensions;
using RapidLogger;
using ReadyApi.Filters;
using ReadyApi.Handlers;
using ReadyApi.Model;

namespace ReadyApi
{
    public static class ReadyApiExtensions
    {
        public static HttpConfiguration UseGlobalExceptionHandler(this HttpConfiguration configuration)
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