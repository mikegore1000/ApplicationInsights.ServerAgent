using System;
using System.Diagnostics.Eventing.Reader;

namespace ApplicationInsights.ServerAgent
{
    public class WindowsEventLogPoller : IEventLogPoller
    {
        private readonly ITelemetrySender sender;
        private readonly EventLogWatcher watcher;

        public WindowsEventLogPoller(string logName, ITelemetrySender sender)
        {
            Guard.IsNotNullOrEmpty(nameof(logName), logName);
            Guard.IsNotNull(nameof(sender), sender);

            this.sender = sender;

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
                }
                // TODO: Think about how to handle these more gracefully
                catch (EventLogException) { }
                catch (UnauthorizedAccessException) { }
            }
        }
    }
}
