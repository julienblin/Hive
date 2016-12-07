using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Hive.Telemetry
{
	public static class TelemetryExtensions
	{
		public static void TrackDependency(
			this ITelemetry telemetry,
			Action action,
			DependencyKind dependencyKind,
			string dependencyName,
			IDictionary<string, string> properties = null,
			[CallerMemberName] string commandName = null)
		{
			var startTime = DateTimeOffset.UtcNow;
			var sw = Stopwatch.StartNew();
			try
			{
				action();
				sw.Stop();
				telemetry.TrackDependency(
					dependencyKind,
					dependencyName,
					commandName,
					startTime,
					sw.Elapsed,
					true,
					null,
					properties);
			}
			catch (Exception ex)
			{
				sw.Stop();
				properties = properties ?? new Dictionary<string, string>();
				properties["Exception"] = ex.ToString();
				telemetry.TrackDependency(
					dependencyKind,
					dependencyName,
					commandName,
					startTime,
					sw.Elapsed,
					false,
					null,
					properties);
				throw;
			}
		}

		public static T TrackDependency<T>(
			this ITelemetry telemetry,
			Func<T> action,
			DependencyKind dependencyKind,
			string dependencyName,
			IDictionary<string, string> properties = null,
			[CallerMemberName] string commandName = null)
		{
			var startTime = DateTimeOffset.UtcNow;
			var sw = Stopwatch.StartNew();
			try
			{
				var result = action();
				sw.Stop();
				telemetry.TrackDependency(
					dependencyKind,
					dependencyName,
					commandName,
					startTime,
					sw.Elapsed,
					true,
					null,
					properties);

				return result;
			}
			catch (Exception ex)
			{
				sw.Stop();
				properties = properties ?? new Dictionary<string, string>();
				properties["Exception"] = ex.ToString();
				telemetry.TrackDependency(
					dependencyKind,
					dependencyName,
					commandName,
					startTime,
					sw.Elapsed,
					false,
					null,
					properties);
				throw;
			}
		}

		public static async Task TrackAsyncDependency(
			this ITelemetry telemetry,
			CancellationToken ct,
			Func<CancellationToken, Task> action,
			DependencyKind dependencyKind,
			string dependencyName,
			IDictionary<string, string> properties = null,
			[CallerMemberName] string commandName = null)
		{
			var startTime = DateTimeOffset.UtcNow;
			var sw = Stopwatch.StartNew();
			try
			{
				await action(ct);
				sw.Stop();
				telemetry.TrackDependency(
					dependencyKind,
					dependencyName,
					commandName,
					startTime,
					sw.Elapsed,
					true,
					null,
					properties);
			}
			catch (Exception ex)
			{
				sw.Stop();
				properties = properties ?? new Dictionary<string, string>();
				properties["Exception"] = ex.ToString();
				telemetry.TrackDependency(
					dependencyKind,
					dependencyName,
					commandName,
					startTime,
					sw.Elapsed,
					false,
					null,
					properties);
				throw;
			}
		}

		public static async Task<T> TrackAsyncDependency<T>(
			this ITelemetry telemetry,
			CancellationToken ct,
			Func<CancellationToken, Task<T>> action,
			DependencyKind dependencyKind,
			string dependencyName,
			IDictionary<string, string> properties = null,
			[CallerMemberName] string commandName = null)
		{
			var startTime = DateTimeOffset.UtcNow;
			var sw = Stopwatch.StartNew();
			try
			{
				var result = await action(ct);
				sw.Stop();
				telemetry.TrackDependency(
					dependencyKind,
					dependencyName,
					commandName,
					startTime,
					sw.Elapsed,
					true,
					null,
					properties);
				return result;
			}
			catch (Exception ex)
			{
				sw.Stop();
				properties = properties ?? new Dictionary<string, string>();
				properties["Exception"] = ex.ToString();
				telemetry.TrackDependency(
					dependencyKind,
					dependencyName,
					commandName,
					startTime,
					sw.Elapsed,
					false,
					null,
					properties);
				throw;
			}
		}
	}
}