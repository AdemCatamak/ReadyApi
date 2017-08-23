using Autofac;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidLogger;

namespace ReadyApi.IntegrationTest.StartupTest
{
    [TestClass]
    public class WebApiConfigTest
    {
        private readonly string baseAddress = "http://localhost:9000/";

        [TestMethod]
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