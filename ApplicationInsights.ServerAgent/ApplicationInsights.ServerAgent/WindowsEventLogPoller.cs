using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ApplicationInsights.ServerAgent
{
    public class WindowsEventLogPoller : IEventLogPoller
    {
        private readonly ITelemetrySender sender;
        private readonly EventLogWatcher watcher;
        private readonly string logName;

        public WindowsEventLogPoller(string logName, ITelemetrySender sender)
        {
            Guard.IsNotNullOrEmpty(nameof(logName), logName);
            Guard.IsNotNull(nameof(sender), sender);

            this.sender = sender;
            this.logName = logName;

            watcher = new EventLogWatcher(new EventLogQuery(logName, PathType.LogName, "*"), null, true);
            watcher.EventRecordWritten += OnEventRecordWritten;
        }

        public void Start()
        {
            watcher.Enabled = true;
        }

        private void OnEventRecordWritten(object sender, EventRecordWrittenEventArgs eventRecordWrittenEventArgs)
        {
            if (eventRecordWrittenEventArgs.EventRecord != null)
            {
                try
                {
                    var trace = TelemetryMapper.ToTrace(eventRecordWrittenEventArgs.EventRecord);
                    this.sender.SendTrace(trace);
                    Bookmark(eventRecordWrittenEventArgs.EventRecord.Bookmark);

                }
                // TODO: Think about how to handle these more gracefully
                catch (EventLogException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        private void Bookmark(EventBookmark bookmark)
        {
            var serializer = new BinaryFormatter();

            using (var writer = File.OpenWrite(BookmarkName))
            {
                serializer.Serialize(writer, bookmark);
            }
        }

        private string BookmarkName => $"{logName.ToLowerInvariant()}-bookmark.txt";
    }
}
