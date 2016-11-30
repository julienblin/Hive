using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hive.Foundation.Extensions
{
	public static class JsonSerializerExtensions
	{
		public static void Serialize(this JsonSerializer serializer, object value, Stream stream)
		{
			serializer.NotNull(nameof(serializer));
			stream.NotNull(nameof(stream));

			using (var streamWriter = new StreamWriter(stream))
			using (var jsonWriter = new JsonTextWriter(streamWriter))
			{
				serializer.Serialize(jsonWriter, value);
			}
		}

		public static T Deserialize<T>(this JsonSerializer serializer, Stream stream)
		{
			serializer.NotNull(nameof(serializer));
			stream.NotNull(nameof(stream));

			using (var streamReader = new StreamReader(stream))
			using (var jsonTextReader = new JsonTextReader(streamReader))
			{
				return serializer.Deserialize<T>(jsonTextReader);
			}
		}

		public static T Deserialize<T>(this JsonSerializer serializer, string path)
		{
			serializer.NotNull(nameof(serializer));
			path.NotNullOrEmpty(nameof(path));

			using (var stream = File.OpenRead(path))
			{
				return serializer.Deserialize<T>(stream);
			}
		}
	}
}