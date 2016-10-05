using System;
using System.Diagnostics.Eventing.Reader;

namespace ApplicationInsights.ServerAgent
{
    public class WindowsEventLogPoller : IEventLogPoller
    {
        private readonly Action<EventRecord> eventPolled;
        private readonly EventLogWatcher watcher;

        public WindowsEventLogPoller(string logName, Action<EventRecord> eventPolled)
        {
            Guard.IsNotNullOrEmpty(nameof(logName), logName);
            Guard.IsNotNull(nameof(eventPolled), eventPolled);

            this.eventPolled = eventPolled;

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
                eventPolled(eventRecordWrittenEventArgs.EventRecord);
            }
        }
    }
}
