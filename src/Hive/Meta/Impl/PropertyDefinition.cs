using System.Collections.Generic;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;

namespace Hive.Meta.Impl
{
	internal class PropertyDefinition : IPropertyDefinition
	{
		public string Name { get; set; }

		public IDataType PropertyType { get; set; }

		public IDictionary<string, object> Properties { get; set; }

		public T GetProperty<T>(string propertyName)
		{
			var result = Properties.SafeGet(propertyName);
			return result == null ? default(T) : (T)result;
		}

		internal PropertyDefinitionData PropertyDefinitionData { get; set; }
	}
}