using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Topshelf;

namespace ApplicationInsights.ServerAgent
{
    class Program
    {
        static void Main()
        {
            HostFactory.Run(x =>
            {
                TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];
                var sender = new TelemetrySender(new TelemetryClient());
                var pollers = CreatePollers(sender);

                x.Service<ServerAgent>(s =>
                {
                    s.ConstructUsing(() => new ServerAgent(pollers));
                    s.WhenStarted(sa => sa.Start());
                    s.WhenStopped(sa => sa.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Appliation Insights Server Agent");
                x.SetDisplayName("Appliation Insights Server Agent");
                x.SetServiceName("ApplicationInsights.ServerAgent");
            });
        }

        private static IEnumerable<IEventLogPoller> CreatePollers(ITelemetrySender sender)
        {
            var logs = ConfigurationManager.AppSettings["LogsToPollFrom"];

            foreach (var l in logs.Replace(" ", string.Empty).Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return new WindowsEventLogPoller(l, sender);
            }
        }
    }

    public class NullEventLogPoller : IEventLogPoller
    {
        public void Start() {}

        public void Dispose() {}
    }
}
