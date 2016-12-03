namespace Hive.Serialization
{
	public interface IEntitySerializerFactory
	{
		IEntitySerializer GetByMediaType(string mediaType);
	}
}