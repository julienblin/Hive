using System;
using System.Collections.Generic;

namespace Hive.Events
{
	public interface IEvent
	{
		Guid Id { get; }

		string Source { get; }

		string Name { get; }

		IDictionary<string, string> Properties { get; }
	}
}