using Autofac;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using RapidLogger;

namespace ReadyApi.UnitTest.StartupTest
{
    
    public class WebApiConfigTest
    {
        private readonly string baseAddress = "http://localhost:9000/";

        [Test]
        public void ValidateIoCContainerRegistered()
        {
            using (WebApp.Start(baseAddress, Startup.Configuration))
            {
                IContainer container = Startup.IoCContainer;

                bool isILoggerRegistered = container.IsRegistered<LoggerMaestro>();
                Assert.IsTrue(isILoggerRegistered);
            }
        }
    }
}