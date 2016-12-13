using System.Collections.Generic;

namespace Hive.Queries
{
	public interface IProjection
	{
		IEnumerable<string> PropertyNames { get; }
	}
}