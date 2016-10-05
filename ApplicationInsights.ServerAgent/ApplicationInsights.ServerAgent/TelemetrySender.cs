using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace ApplicationInsights.ServerAgent
{
    public interface ITelemetrySender
    {
        void SendTrace(TraceTelemetry trace);
    }

    public class TelemetrySender : ITelemetrySender
    {
        private readonly TelemetryClient client;

        public TelemetrySender(TelemetryClient client)
        {
            this.client = client;
        }

        public void SendTrace(TraceTelemetry trace)
        {
            client.TrackTrace(trace);
        }
    }
}
