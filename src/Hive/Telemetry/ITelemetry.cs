using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hive.Telemetry
{
	public interface ITelemetry
	{
		void TrackTrace(string prefix, string message, IDictionary<string, string> properties = null,
			[CallerMemberName] string from = null);

		void TrackWarn(string prefix, string message, IDictionary<string, string> properties = null,
			[CallerMemberName] string from = null);

		void TrackEvent(string prefix, string @event, IDictionary<string, string> properties = null,
			[CallerMemberName] string from = null);

		void TrackException(Exception exception, IDictionary<string, string> properties = null,
			[CallerMemberName] string from = null);

		void TrackDependency(
			DependencyKind dependencyKind,
			string dependencyName,
			string commandName,
			DateTimeOffset startTime,
			TimeSpan duration,
			bool success,
			string resultCode = null,
			IDictionary<string, string> properties = null);
	}
}