using Hive.Foundation.Extensions;

namespace Hive.Queries
{
	public class Order
	{
		private readonly bool _ascending;

		private readonly string _propertyName;

		public Order(string propertyName, bool ascending)
		{
			_propertyName = propertyName.NotNullOrEmpty(nameof(propertyName));
			_ascending = ascending;
		}

		public static Order Asc(string propertyName)
		{
			return new Order(propertyName, true);
		}

		public static Order Desc(string propertyName)
		{
			return new Order(propertyName, false);
		}
	}
}