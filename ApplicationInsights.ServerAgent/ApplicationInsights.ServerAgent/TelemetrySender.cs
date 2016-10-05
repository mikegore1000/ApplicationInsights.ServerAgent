using System;
using Microsoft.ApplicationInsights.DataContracts;

namespace ApplicationInsights.ServerAgent
{
    public interface ITelemetrySender
    {
        void SendTrace(TraceTelemetry trace);
    }

    public class TelemetrySender : ITelemetrySender
    {
        public void SendTrace(TraceTelemetry trace)
        {
            throw new NotImplementedException();
        }
    }
}
