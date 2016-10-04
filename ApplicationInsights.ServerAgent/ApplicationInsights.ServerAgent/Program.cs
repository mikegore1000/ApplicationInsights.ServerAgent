using System;
using System.Collections.Generic;
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

    public class ServerAgent
    {
        private readonly IEnumerable<IEventLogPoller> pollers;
        private bool started;

        public ServerAgent(IEnumerable<IEventLogPoller> pollers)
        {
            this.pollers = pollers;
            this.started = false;
        }

        public void Start()
        {
            if (started)
            {
                return;
            }

            foreach (var p in pollers)
            {
                p.Start();
            }

            started = true;
        }

        public void Stop()
        {
        }
    }

    public interface IEventLogPoller : IDisposable
    {
        void Start();
    }
}
