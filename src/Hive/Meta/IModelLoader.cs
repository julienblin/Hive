using Hive.Foundation.Entities;

namespace Hive.Meta
{
	public interface IModelLoader
	{
		IModel Load(PropertyBag modelData);
	}
}