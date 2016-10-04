using System.Configuration;
using Microsoft.ApplicationInsights.Extensibility;
using Topshelf;

namespace ApplicationInsights.ServerAgent
{
    class Program
    {
        static void Main()
        {
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];

            HostFactory.Run(x =>
            {
                x.Service<ServerAgent>(s =>
                {
                    s.ConstructUsing(() => new ServerAgent(new[] { new NullEventLogPoller() }));
                    s.WhenStarted(sa => sa.Start());
                    s.WhenStopped(sa => sa.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Appliation Insights Server Agent");
                x.SetDisplayName("Appliation Insights Server Agent");
                x.SetServiceName("ApplicationInsights.ServerAgent");
            });
        }
    }

    public class NullEventLogPoller : IEventLogPoller
    {
        public void Start() {}

        public void Dispose() {}
    }
}
