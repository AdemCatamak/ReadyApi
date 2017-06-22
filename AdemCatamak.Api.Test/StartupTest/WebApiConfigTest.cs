using AdemCatamak.Logger;
using Autofac;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdemCatamak.Api.Test.StartupTest
{
    [TestClass]
    public class WebApiConfigTest
    {
        private readonly string baseAddress = "http://localhost:9000/";

        [TestMethod]
        public void ValidateIoCContainerRegistered()
        {
            using (WebApp.Start<Startup>(baseAddress))
            {
                IContainer container = Startup.IoCContainer;

                bool isILoggerRegistered = container.IsRegistered<ILogWrapper>();
                Assert.IsTrue(isILoggerRegistered);
            }
        }
    }
}