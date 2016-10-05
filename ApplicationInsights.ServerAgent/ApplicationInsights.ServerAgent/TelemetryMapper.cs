using System;
using System.Diagnostics.Eventing.Reader;
using Microsoft.ApplicationInsights.DataContracts;

namespace ApplicationInsights.ServerAgent
{
    internal static class TelemetryMapper
    {
        internal static TraceTelemetry ToTrace(EventRecord @event)
        {
            var trace = new TraceTelemetry(@event.FormatDescription(), MapSeverity(@event.Level));
            trace.Timestamp = @event.TimeCreated.GetValueOrDefault(DateTime.UtcNow);
            trace.Context.Cloud.RoleInstance = @event.MachineName;
            trace.Context.Device.Id = @event.MachineName;
            trace.Context.Device.OperatingSystem = Environment.OSVersion.VersionString;
            trace.Context.Properties.Add("LogName", @event.LogName);

            return trace;
        }

        private static SeverityLevel MapSeverity(byte? level)
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