using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationInsights.ServerAgent
{
    public class ServerAgent
    {
        private readonly IEnumerable<IEventLogPoller> pollers;
        private bool started;
        private CancellationTokenSource cancellationTokenSource;

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

            var taskFactory = new TaskFactory();
            cancellationTokenSource = new CancellationTokenSource();

            foreach (var p in pollers)
            {
                taskFactory.StartNew(() => p.Start(), cancellationTokenSource.Token);
            }

            started = true;
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }
    }
}