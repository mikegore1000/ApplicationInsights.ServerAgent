using System;
using System.Diagnostics.Eventing;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ApplicationInsights.ServerAgent.Tests
{
    public class EventLogPollerTests
    {
        [Fact]
        public void when_started_events_are_streamed()
        {
            EventRecord capturedEvent = null;
            var resetEvent = new ManualResetEventSlim();
            var sut = new WindowsEventLogPoller("Application", e =>
            {
                capturedEvent = e;
                resetEvent.Set();
            });

            sut.Start();

            resetEvent.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            Assert.NotNull(capturedEvent);
        }
    }
}
