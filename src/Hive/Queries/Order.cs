using Hive.Foundation.Extensions;

namespace Hive.Queries
{
	public class Order
	{
		public static Order Asc(string propertyName)
		{
			return new Order(propertyName, true);
		}

		public static Order Desc(string propertyName)
		{
			return new Order(propertyName, false);
		}

		public Order(string propertyName, bool ascending)
		{
			PropertyName = propertyName.NotNullOrEmpty(nameof(propertyName));
			Ascending = ascending;
		}

		public string PropertyName { get; }

		public bool Ascending { get; }
	}
}