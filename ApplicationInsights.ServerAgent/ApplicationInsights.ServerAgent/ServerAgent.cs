using System.Collections.Generic;

namespace ApplicationInsights.ServerAgent
{
    public class ServerAgent
    {
        private readonly IEnumerable<IEventLogPoller> pollers;
        private bool started;

        public ServerAgent(IEnumerable<IEventLogPoller> pollers)
        {
            this.pollers = pollers;
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
}