using System;
using System.Diagnostics.Eventing;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Xunit;

namespace ApplicationInsights.ServerAgent.Tests
{
    public class EventLogPollerTests
    {
        [Fact]
        public void when_started_events_are_streamed_to_the_telemetry_sender()
        {
            TraceTelemetry capturedEvent = null;
            var resetEvent = new ManualResetEventSlim();
            var sender = new FakeTelemetrySender(e =>
            {
                capturedEvent = e;
                resetEvent.Set();
            });
            
            var sut = new WindowsEventLogPoller("Application", sender);

            sut.Start();

            resetEvent.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            Assert.NotNull(capturedEvent);
        }

        [Fact]
        public void when_started_events_are_bookmarked()
        {
            File.Delete("application-bookmark.txt");

            var resetEvent = new ManualResetEventSlim();
            var sender = new FakeTelemetrySender(e =>
            {
                resetEvent.Set();
            });

            var sut = new WindowsEventLogPoller("Application", sender);

            sut.Start();

            resetEvent.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            Assert.True(File.Exists("application-bookmark.txt"), "bookmark file should have been created");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void when_an_invalid_event_log_name_is_provided_an_exception_is_thrown(string logName)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new WindowsEventLogPoller(logName, new FakeTelemetrySender(null));
            });
        }

        [Fact]
        public void when_an_invalid_telemetry_sender_is_provided_an_exception_is_thrown()
        {
            Assert.Throws<ArgumentNullException>(() => new WindowsEventLogPoller("Application", null));
        }

        private class FakeTelemetrySender : ITelemetrySender
        {
            private readonly Action<TraceTelemetry> onSendTrace;

            public FakeTelemetrySender(Action<TraceTelemetry> onSendTrace)
            {
                this.onSendTrace = onSendTrace;
            }

            public void SendTrace(TraceTelemetry trace)
            {
                onSendTrace(trace);
            }
        }
    }
}
