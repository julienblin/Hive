namespace Hive.Foundation.Exceptions
{
	public class NotFoundException : HiveException
	{
		public NotFoundException(string message)
			: base(message)
		{
		}
	}
}