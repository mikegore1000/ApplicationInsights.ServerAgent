using System;
using System.Diagnostics.Eventing;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;
using Xunit;

namespace ApplicationInsights.ServerAgent.Tests
{
    public class EventLogPollerTests
    {
        // TODO: Need to make this test more deterministic
        [Fact]
        public async Task when_started_events_are_streamed()
        {
            EventRecord capturedEvent = null;
            var sut = new WindowsEventLogPoller("Application", e => capturedEvent = e);

            sut.Start();

            await Task.Delay(TimeSpan.FromSeconds(0.5));

            Assert.NotNull(capturedEvent);
        }
    }
}
