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
            
            var sut = new WindowsEventLogPoller("Application", sender, new FakeBookmarker());

            sut.Start();

            resetEvent.Wait(TimeSpan.FromSeconds(1));
            Assert.NotNull(capturedEvent);
        }

        [Fact]
        public void when_started_events_are_bookmarked()
        {
            var resetEvent = new ManualResetEventSlim();
            var sender = new FakeTelemetrySender();
            var bookmarker = new FakeBookmarker(() => resetEvent.Set());

            var sut = new WindowsEventLogPoller("Application", sender, bookmarker);

            sut.Start();

            resetEvent.Wait(TimeSpan.FromSeconds(10));
            Assert.NotNull(bookmarker.GetLatest("Application"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void when_an_invalid_event_log_name_is_provided_an_exception_is_thrown(string logName)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new WindowsEventLogPoller(logName, new FakeTelemetrySender(), new FakeBookmarker());
            });
        }

        [Fact]
        public void when_an_invalid_telemetry_sender_is_provided_an_exception_is_thrown()
        {
            Assert.Throws<ArgumentNullException>(() => new WindowsEventLogPoller("Application", null, new FakeBookmarker()));
        }

        private class FakeBookmarker : IBookmarker
        {
            private EventBookmark latest;
            private Action onBookmark;

            public FakeBookmarker(Action onBookmark = null)
            {
                this.onBookmark = onBookmark;
            }

            public void Bookmark(EventBookmark bookmark, string bookmarkName)
            {
                latest = bookmark;

                onBookmark?.Invoke();
            }

            public EventBookmark GetLatest(string bookmarkName)
            {
                return latest;
            }
        }

        private class FakeTelemetrySender : ITelemetrySender
        {
            private readonly Action<TraceTelemetry> onSendTrace;

            public FakeTelemetrySender(Action<TraceTelemetry> onSendTrace = null)
            {
                this.onSendTrace = onSendTrace;
            }

            public void SendTrace(TraceTelemetry trace)
            {
                onSendTrace?.Invoke(trace);
            }
        }
    }
}
