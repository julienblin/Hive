using Hive.Foundation.Extensions;

namespace Hive.Meta
{
	public class ResourceDescription
	{
		public ResourceDescription(string singleName, string pluralName)
		{
			SingleName = singleName.NotNullOrEmpty(nameof(singleName));
			PluralName = pluralName.NotNullOrEmpty(nameof(pluralName));
		}

		public string SingleName { get; }

		public string PluralName { get; }
	}
}