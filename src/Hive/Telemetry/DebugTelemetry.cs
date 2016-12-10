using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hive.Context;

namespace Hive.Telemetry
{
	public class DebugTelemetry : ITelemetry
	{
		private readonly IContextService _contextService;

		public DebugTelemetry(IContextService contextService)
		{
			_contextService = contextService;
		}

		public void TrackTrace(string prefix, string message, IDictionary<string, string> properties = null,
			string from = null)
		{
			Debug.WriteLine($"Trace: [{from}] {prefix}:{message} {Format(properties)} ({_contextService.GetContext().OperationId})");
		}

		public void TrackWarn(string prefix, string message, IDictionary<string, string> properties = null, string from = null)
		{
			Debug.WriteLine($"Warn: [{from}] {prefix}:{message} {Format(properties)} ({_contextService.GetContext().OperationId})");
		}

		public void TrackEvent(string prefix, string @event, IDictionary<string, string> properties = null, string from = null)
		{
			Debug.WriteLine($"Event: [{from}] {prefix}:{@event} {Format(properties)} ({_contextService.GetContext().OperationId})");
		}

		public void TrackException(Exception exception, IDictionary<string, string> properties = null, string from = null)
		{
			Debug.WriteLine($"Exception: [{from}] {exception} {Format(properties)} ({_contextService.GetContext().OperationId})");
		}

		public void TrackDependency(DependencyKind dependencyKind, string dependencyName, string commandName,
			DateTimeOffset startTime,
			TimeSpan duration, bool success, string resultCode = null, IDictionary<string, string> properties = null)
		{
			Debug.WriteLine(
				$"Dependency: [{dependencyName}] {commandName} (Success: {success}, Duration: {duration}) {Format(properties)} ({_contextService.GetContext().OperationId})");
		}

		private string Format(IDictionary<string, string> properties)
		{
			return properties == null ? null : string.Join(",", properties.Select(x => $"{x.Key}: {x.Value}"));
		}
	}
}