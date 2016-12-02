using System.Collections.Generic;

namespace Hive.Meta.Data
{
	public abstract class PropertyDefinitionData
	{
		public string Name { get; set; }

		public string Type { get; set; }

		public abstract T GetValue<T>(string propertyName);
	}
}