using System;
using System.Configuration;
using Microsoft.Owin.Hosting;
using ReadyApi;

namespace $rootnamespace$
{
    public class Program
    {
        public static void Main()
        {
            string baseAddress = ConfigurationManager.AppSettings["BaseAddress"];

            using (WebApp.Start(baseAddress, Startup.Configuration))
            {
                Console.WriteLine("Press 'E' for exit");
                string read;
                do
                {
                    read = Console.ReadLine();
                } while (read != "E");
            }
        }
    }
}