using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Hive.Meta.Data;
using Hive.Meta.Impl;

namespace Hive.Meta
{
	public interface IValueType : IDataType
	{
		IDictionary<string, object> LoadAdditionalProperties(IValueTypeFactory valueTypeFactory, IModel model, PropertyDefinitionData propertyDefinitionData);
	}
}
