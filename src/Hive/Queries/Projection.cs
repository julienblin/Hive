using Hive.Meta;
using Hive.Queries.Projections;

namespace Hive.Queries
{
	public static class Projection
	{
		public static IProjection Id() => new PropertiesProjection(MetaConstants.IdProperty);

		public static IProjection Properties(params string[] propertyNames) => new PropertiesProjection(propertyNames);
	}
}