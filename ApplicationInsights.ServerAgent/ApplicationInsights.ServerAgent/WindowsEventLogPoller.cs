using System;
using System.Diagnostics.Eventing.Reader;
using Microsoft.ApplicationInsights.DataContracts;

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
                    var trace = CreateTraceTelemetry(eventRecordWrittenEventArgs.EventRecord);
                    this.sender.SendTrace(trace);
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        private TraceTelemetry CreateTraceTelemetry(EventRecord @event)
        {
            var trace =  new TraceTelemetry(@event.FormatDescription(), MapSeverity(@event.Level));
            trace.Timestamp = @event.TimeCreated.GetValueOrDefault(DateTime.UtcNow);
            trace.Context.Cloud.RoleInstance = @event.MachineName;
            trace.Context.Device.Id = @event.MachineName;
            trace.Context.Device.OperatingSystem = Environment.OSVersion.VersionString;

            return trace;
        }

        private SeverityLevel MapSeverity(byte? level)
        {
            if (!level.HasValue)
                return SeverityLevel.Information;

            switch (level.Value)
            {
                case 1:
                    return SeverityLevel.Critical;
                case 2:
                    return SeverityLevel.Error;
                case 3:
                    return SeverityLevel.Warning;
                case 5:
                    return SeverityLevel.Verbose;
                default:
                    return SeverityLevel.Information;
            }
        }
    }
}
