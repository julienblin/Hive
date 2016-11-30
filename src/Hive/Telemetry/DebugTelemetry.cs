using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hive.Telemetry
{
	public class DebugTelemetry : ITelemetry
	{
		public void TrackTrace(string prefix, string message, IDictionary<string, string> properties = null, string @from = null)
		{
			Debug.WriteLine($"Trace: [{from}] {prefix}:{message} {Format(properties)}");
		}

		public void TrackWarn(string prefix, string message, IDictionary<string, string> properties = null, string @from = null)
		{
			Debug.WriteLine($"Warn: [{from}] {prefix}:{message} {Format(properties)}");
		}

		public void TrackEvent(string prefix, string @event, IDictionary<string, string> properties = null, string @from = null)
		{
			Debug.WriteLine($"Event: [{from}] {prefix}:{@event} {Format(properties)}");
		}

		public void TrackException(Exception exception, IDictionary<string, string> properties = null, string @from = null)
		{
			Debug.WriteLine($"Exception: [{from}] {exception} {Format(properties)}");
		}

		public void TrackDependency(DependencyKind dependencyKind, string dependencyName, string commandName, DateTimeOffset startTime,
			TimeSpan duration, bool success, string resultCode = null, IDictionary<string, string> properties = null)
		{
			Debug.WriteLine($"Dependency: [{dependencyName}] {commandName} (Success: {success}, Duration: {duration}) {Format(properties)}");
		}

		private string Format(IDictionary<string, string> properties)
		{
			return properties == null ? null : string.Join(",", properties.Select(x => $"{x.Key}: {x.Value}"));
		}
	}
}