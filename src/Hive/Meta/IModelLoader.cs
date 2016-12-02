using Hive.Meta.Data;

namespace Hive.Meta
{
	public interface IModelLoader
	{
		IModel Load(ModelData modelData);
	}
}