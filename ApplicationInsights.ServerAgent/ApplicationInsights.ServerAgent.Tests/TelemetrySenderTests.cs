using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Xunit;

namespace ApplicationInsights.ServerAgent.Tests
{
    public class TelemetrySenderTests
    {
        [Fact]
        public void when_sending_trace_telemetry_no_errors_occur()
        {
            var fakeChannel = new FakeTelemetryChannel();
            var config = new TelemetryConfiguration
            {
                TelemetryChannel = fakeChannel,
                InstrumentationKey = "FAKE_KEY"
            };
            var client = new TelemetryClient(config);

            var sut = new TelemetrySender(client);
            var trace = new TraceTelemetry("Test", SeverityLevel.Information);

            sut.SendTrace(trace);

            var sentTrace = fakeChannel.CapturedTelemetry.OfType<TraceTelemetry>().Single();
            Assert.Equal(trace, sentTrace);
        }

        private class FakeTelemetryChannel : ITelemetryChannel
        {
            private readonly List<ITelemetry> capturedTelemetry = new List<ITelemetry>();

            public IEnumerable<ITelemetry> CapturedTelemetry => capturedTelemetry;

            public void Dispose()
            {
            }

            public void Send(ITelemetry item)
            {
                capturedTelemetry.Add(item);
            }

            public void Flush()
            {
            }

            public bool? DeveloperMode { get; set; }
            public string EndpointAddress { get; set; }
        }
    }
}
