using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Topshelf;

namespace ApplicationInsights.ServerAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ServerAgent>(s =>
                {
                    s.ConstructUsing(() => new ServerAgent());
                    s.WhenStarted(sa => sa.Start());
                    s.WhenStopped(sa => sa.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Appliation Insights Server Agent");
                x.SetDisplayName("Appliation Insights Server Agent");
                x.SetServiceName("ApplicationInsights.ServerAgent");
            });

            Console.ReadLine();
        }

        public class ServerAgent
        {
            public void Start()
            {
                TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];
            }

            public void Stop()
            {
                
            }
        }
    }
}
