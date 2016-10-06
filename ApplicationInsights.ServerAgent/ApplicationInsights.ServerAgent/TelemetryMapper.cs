using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
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
            trace.Context.Properties.Add("LogName", @event.LogName);
            trace.Context.Properties.Add("MachineName", @event.MachineName);
            trace.Context.Properties.Add("ProviderName", @event.ProviderName);
            trace.Context.Properties.Add("ActivityId", @event.ActivityId.GetValueOrDefault(Guid.Empty).ToString());
            trace.Context.Properties.Add("EventId", @event.Id.ToString());
            trace.Context.Properties.Add("KeywordsDisplayName", string.Join(", ", @event.KeywordsDisplayNames));
            trace.Context.Properties.Add("ProcessId", @event.ProcessId.GetValueOrDefault(0).ToString());
            trace.Context.Properties.Add("ProviderId", @event.ProviderId.ToString());

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