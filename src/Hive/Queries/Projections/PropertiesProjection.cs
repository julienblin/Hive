using System.Collections.Generic;

namespace Hive.Queries.Projections
{
	public class PropertiesProjection : IProjection
	{
		public IEnumerable<string> PropertyNames { get; }

		public PropertiesProjection(params string[] propertyNames)
		{
			PropertyNames = propertyNames;
		}
	}
}