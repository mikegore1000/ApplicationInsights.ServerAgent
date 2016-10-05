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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void when_an_invalid_event_log_name_is_provided_an_exception_is_thrown(string logName)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new WindowsEventLogPoller(logName, e => { });
            });
        }

        [Fact]
        public void when_an_invalid_callback_is_provided_an_exception_is_thrown()
        {
            Assert.Throws<ArgumentNullException>(() => new WindowsEventLogPoller("Application", null));
        }
    }
}
