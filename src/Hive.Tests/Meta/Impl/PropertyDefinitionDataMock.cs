using System.Collections.Generic;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;

namespace Hive.Tests.Meta.Impl
{
	public class PropertyDefinitionDataMock : PropertyDefinitionData
	{
		public PropertyDefinitionDataMock(string name, string type, IDictionary<string, object> values = null)
		{
			Name = name;
			Type = type;
			Values = values ?? new Dictionary<string, object>();
		}

		public IDictionary<string, object> Values { get; set; }

		public override T GetValue<T>(string propertyName)
		{
			var result = Values.SafeGet(propertyName);
			return result == null ? default(T) : (T)result;
		}
	}
}