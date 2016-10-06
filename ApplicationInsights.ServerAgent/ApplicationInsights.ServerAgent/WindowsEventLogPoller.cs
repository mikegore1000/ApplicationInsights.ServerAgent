using System;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationInsights.ServerAgent
{
    public class WindowsEventLogPoller : IEventLogPoller
    {
        private readonly ITelemetrySender sender;
        private readonly EventLogWatcher watcher;
        private readonly string logName;
        private readonly IBookmarker bookmarker;

        public WindowsEventLogPoller(string logName, ITelemetrySender sender, IBookmarker bookmarker)
        {
            Guard.IsNotNullOrEmpty(nameof(logName), logName);
            Guard.IsNotNull(nameof(sender), sender);

            this.sender = sender;
            this.logName = logName;
            this.bookmarker = bookmarker;

            watcher = new EventLogWatcher(new EventLogQuery(logName, PathType.LogName, "*"), this.bookmarker.GetLatest(BookmarkName), true);
            watcher.EventRecordWritten += OnEventRecordWritten;
        }

        public void Start()
        {
            new TaskFactory().StartNew(() => watcher.Enabled = true);
        }

        private void OnEventRecordWritten(object sender, EventRecordWrittenEventArgs eventRecordWrittenEventArgs)
        {
            if (eventRecordWrittenEventArgs.EventRecord != null)
            {
                try
                {
                    var trace = TelemetryMapper.ToTrace(eventRecordWrittenEventArgs.EventRecord);
                    this.sender.SendTrace(trace);
                    bookmarker.Bookmark(eventRecordWrittenEventArgs.EventRecord.Bookmark, BookmarkName);

                }
                // TODO: Think about how to handle these more gracefully
                catch (EventLogException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        private string BookmarkName => $"{logName.ToLowerInvariant()}-bookmark";
    }
}
