using System;

namespace ApplicationInsights.ServerAgent
{
    public interface IEventLogPoller : IDisposable
    {
        void Start();
    }
}